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

using Microsoft.Extensions.Caching.Memory;

namespace BpnTrade.App.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        private readonly IPaymentFacade _paymentFacade;
        private readonly ICancelAdapter _cancelAdapter;

        public OrderService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IMemoryCache memoryCache,
            IPaymentFacade paymentFacade,
            ICancelAdapter cancelAdapter)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
            _paymentFacade = paymentFacade;
            _cancelAdapter = cancelAdapter;
        }

        public async Task<ResultDto<CompleteOrderResponseDto>> CompleteOrderAsync(CompleteOrderRequestDto dto, CancellationToken cancellationToken = default)
        {
            var orderRepository = _unitOfWork.GetRepository<IOrderRepository>();

            var entity = await orderRepository.GetAsync(dto.OrderId);

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

            var paymentResult = await _paymentFacade.ProcessPayment(new ProcessPaymentRequestDto()
            {
                Amount = entity.OrderItems.Sum(x => x.Quantity * x.UnitPrice),
                OrderId = entity.Id.ToString(),
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

                if (!cancelResult.IsSuccess)
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
            if (!ValidateProductsExists(dto))
                return ResultRoot.Failure<CreateOrderResponseDto>(new ErrorDto("PRD002", "Bir ya da daha çok ürün bilgisi bulunamadı"));

            var orderRepository = _unitOfWork.GetRepository<IOrderRepository>();

            var entity = _mapper.Map<CreateOrderRequestDto, OrderEntity>(dto);

            await orderRepository.CreateAsync(entity, cancellationToken);

            await _unitOfWork.SaveAsync(cancellationToken);

            var resultDto = new CreateOrderResponseDto
            {
                Id = entity.Id
            };

            return ResultRoot.Success(resultDto);
        }

        private bool ValidateProductsExists(CreateOrderRequestDto dto)
        {
            if (_memoryCache.TryGetValue("Products", out List<ProductResponseDto> _products) && _products.Any())
            {
                foreach (var orderItem in dto.OrderItems)
                {
                    var existsAndValid =
                        _products
                        .SelectMany(x => x.Data)
                        .Any(x =>
                                x.Id == orderItem.ProductId.ToString()
                                &&
                                x.Currency == dto.Currency
                                &&
                                x.Price > 0);

                    if (!existsAndValid)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
