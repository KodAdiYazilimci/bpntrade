using AutoMapper;

using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;
using BpnTrade.Domain.Dto.Order;
using BpnTrade.Domain.Dto.Payment;
using BpnTrade.Domain.Entities;
using BpnTrade.Domain.Facades;
using BpnTrade.Domain.Persistence;
using BpnTrade.Domain.Repositories.EF;
using BpnTrade.Domain.Roots;
using BpnTrade.Domain.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BpnTrade.App.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        private readonly IPaymentFacade _paymentFacade;
        private readonly IPreOrderAdapter _preOrderAdapter;
        private readonly ICancelAdapter _cancelAdapter;
        private readonly IProductAdapter _productAdapter;

        public OrderService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IMemoryCache memoryCache,
            IPaymentFacade paymentFacade,
            ICancelAdapter cancelAdapter,
            IPreOrderAdapter preOrderAdapter,
            IProductAdapter productAdapter)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
            _paymentFacade = paymentFacade;
            _cancelAdapter = cancelAdapter;
            _preOrderAdapter = preOrderAdapter;
            _productAdapter = productAdapter;
        }

        public async Task<ResultDto<CompleteOrderResponseDto>> CompleteOrderAsync(CompleteOrderRequestDto dto, CancellationToken cancellationToken = default)
        {
            var orderRepository = _unitOfWork.GetRepository<IOrderRepository>();

            var entity = await orderRepository.GetAsync(Convert.ToInt32(dto.OrderId));

            if (entity == null)
            {
                return ResultRoot.Failure<CompleteOrderResponseDto>(new ErrorDto("ORD001", "Sipariş bulunamadı"));
            }

            if (!string.Equals(entity.UserId, dto.UserId, StringComparison.OrdinalIgnoreCase))
            {
                return ResultRoot.Failure<CompleteOrderResponseDto>(new ErrorDto("ORD001", "Geçersiz sipariş"));
            }

            if (entity.PaymentDate.HasValue)
            {
                return ResultRoot.Failure<CompleteOrderResponseDto>(new ErrorDto("PAY001", "Sipariş zaten ödenmiş"));
            }

            var orderItems =
                await
                _unitOfWork
                .Context
                .Set<OrderItemEntity>()
                .Where(x => x.OrderId == Convert.ToInt32(dto.OrderId) && x.DeleteDate == null)
                .AsNoTracking()
                .Select(x => new { x.Quantity, x.UnitPrice })
                .ToListAsync(cancellationToken);

            var totalAmount = orderItems.Sum(x => x.Quantity * x.UnitPrice);

            if (totalAmount < 0)
            {
                return ResultRoot.Failure<CompleteOrderResponseDto>(new ErrorDto("BSKT001", "Sipariş tutarı geçersiz"));
            }

            var paymentResult = await _paymentFacade.ProcessPayment(new ProcessPaymentRequestDto()
            {
                Amount = totalAmount,
                OrderId = entity.Id,
                UserId = dto.UserId
            }, cancellationToken);

            var resultDto = new CompleteOrderResponseDto();

            if (paymentResult.IsSuccess)
            {
                entity.PaymentDate = DateTime.Now;

                var dbResult = await _unitOfWork.SaveAsync(cancellationToken);

                if (dbResult.IsSuccess)
                {
                    return ResultRoot.Success(resultDto);
                }

                resultDto.Error = new ErrorDto("DB", "Beklenmeyen bir hata oluştu", $"{dbResult.Error?.Code}", $"{dbResult.Error?.Message}");

                var cancelResult = await _cancelAdapter.CancelAsync(new CancelRequestDto()
                {
                    OrderId = entity.Id.ToString()
                }, cancellationToken);

                if (!cancelResult.IsSuccess || !cancelResult.Data.Success)
                {
                    resultDto.Error = new ErrorDto(cancelResult.Error.Code, cancelResult.Error.Message, resultDto.Error);
                }
            }
            else
            {
                resultDto.Error = paymentResult.Error;
            }

            return ResultRoot.Failure<CompleteOrderResponseDto>(paymentResult.Error);
        }

        public async Task<ResultDto<CreateOrderResponseDto>> CreateAsync(CreateOrderRequestDto dto, CancellationToken cancellationToken = default)
        {
            var validateResult = await ValidateProductsExists(dto, cancellationToken);

            if (!validateResult.IsSuccess)
                return ResultRoot.Failure<CreateOrderResponseDto>(validateResult.Error);

            if (!validateResult.Data)
                return ResultRoot.Failure<CreateOrderResponseDto>(new ErrorDto("PRD002", "Bir ya da daha çok ürün bilgisi bulunamadı"));

            var assignProductPriceResult = await AssignProductPrices(dto, cancellationToken);

            if (!assignProductPriceResult.IsSuccess)
            {
                return ResultRoot.Failure<CreateOrderResponseDto>(assignProductPriceResult.Error);
            }

            var orderRepository = _unitOfWork.GetRepository<IOrderRepository>();

            var entity = _mapper.Map<CreateOrderRequestDto, OrderEntity>(dto);

            await orderRepository.CreateAsync(entity, cancellationToken);

            await _unitOfWork.SaveAsync(cancellationToken);

            var preOrderResult = await _preOrderAdapter.PreOrderAsync(new PreOrderRequestDto()
            {
                Amount = dto.OrderItems.Sum(x => x.Quantity * x.UnitPrice),
                OrderId = entity.Id.ToString()
            }, cancellationToken);

            if (!preOrderResult.IsSuccess || !preOrderResult.Data.Success)
            {
                return ResultRoot.Failure<CreateOrderResponseDto>(new ErrorDto("PRE001", "Ön sipariş başarısız oldu", preOrderResult.Error));
            }

            var resultDto = new CreateOrderResponseDto
            {
                Id = entity.Id
            };

            return ResultRoot.Success(resultDto);
        }

        private async Task<ResultDto<bool>> ValidateProductsExists(CreateOrderRequestDto dto, CancellationToken cancellationToken = default)
        {
            var products = await GetProductsAsync(cancellationToken);

            if (!products.IsSuccess || !products.Data.Success)
            {
                return ResultRoot.Failure<bool>(products.Error);
            }

            foreach (var orderItem in dto.OrderItems)
            {
                var existsAndValid =
                    products
                    .Data
                    ?.Data
                    ?.Any(x =>
                            x.Id == orderItem.ProductId.ToString()
                            &&
                            x.Currency == dto.Currency
                            &&
                            x.Price > 0);

                if (!(existsAndValid ?? false))
                {
                    return ResultRoot.Success(false);
                }
            }

            return ResultRoot.Success(true);
        }

        private async Task<ResultDto> AssignProductPrices(CreateOrderRequestDto dto, CancellationToken cancellationToken = default)
        {
            var products = await GetProductsAsync(cancellationToken);

            if (!products.IsSuccess || !products.Data.Success)
            {
                return ResultRoot.Failure(products.Error);
            }

            foreach (var orderItem in dto.OrderItems)
            {
                orderItem.UnitPrice = products.Data.Data.Where(x => x.Id == orderItem.ProductId).FirstOrDefault()?.Price ?? 0;

                if (orderItem.UnitPrice == 0)
                {
                    return ResultRoot.Failure(new ErrorDto("PRC001", "Ürün fiyatı bulunamadı"));
                }
            }

            return ResultRoot.Success();
        }

        private async Task<ResultDto<ProductResponseDto>> GetProductsAsync(CancellationToken cancellationToken = default)
        {
            if (_memoryCache.TryGetValue("Products", out ProductResponseDto _productResponse) && _productResponse != null)
            {
                return ResultRoot.Success(_productResponse);
            }

            var productResponse = await _productAdapter.GetProductsAsync(cancellationToken);

            if (productResponse != null && productResponse.IsSuccess && productResponse.Data.Success && productResponse.Data.Data != null)
            {
                _memoryCache.Set<ProductResponseDto>("Products", productResponse.Data, TimeSpan.FromSeconds(30));
            }
            else if (productResponse != null && (!productResponse.IsSuccess || !productResponse.Data.Success))
            {
                return ResultRoot.Failure<ProductResponseDto>(productResponse.Error);
            }

            return ResultRoot.Failure<ProductResponseDto>(new ErrorDto("PRD004", "Ürün bilgileri çekilemedi"));
        }
    }
}
