using AutoMapper;

using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Order;
using BpnTrade.Domain.Entities.Integration;
using BpnTrade.Domain.Entities.Persistence;
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

        public OrderService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IMemoryCache memoryCache)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
        }

        public async Task<ResultDto<CompleteOrderResponseDto>> CompleteOrderAsync(CompleteOrderRequestDto dto, CancellationToken cancellationToken = default)
        {
            var orderRepository = _unitOfWork.GetRepository<IOrderRepository>();

            var entity = await orderRepository.GetAsync(dto.OrderId);

            if (entity == null)
            {
                return ResultRoot.Failure<CompleteOrderResponseDto>(new ErrorDto("ORD001", "Sipariş bulunamadı"));
            }

            if (entity.PaymentDate.HasValue)
            {
                return ResultRoot.Failure<CompleteOrderResponseDto>(new ErrorDto("PAY001", "Sipariş zaten ödenmiş"));
            }

            entity.PaymentDate = DateTime.Now;

            await _unitOfWork.SaveAsync(cancellationToken);

            var resultDto = new CompleteOrderResponseDto();

            return ResultRoot.Success(resultDto);
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
            return
                _memoryCache.TryGetValue("Products", out List<ProductEntity> _products)
                &&
                _products != null
                &&
                _products.TrueForAll(x => dto.OrderItems.Select(x => x.ProductId).Contains(int.Parse(x.Id)));
        }
    }
}
