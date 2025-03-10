using AutoMapper;

using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Order;
using BpnTrade.Domain.Entities;
using BpnTrade.Domain.Persistence;
using BpnTrade.Domain.Repositories.EF;
using BpnTrade.Domain.Roots;
using BpnTrade.Domain.Services;

namespace BpnTrade.App.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IMapper mapper, 
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
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

            entity.PaymentDate  = DateTime.Now;

            await _unitOfWork.SaveAsync(cancellationToken);

            var resultDto = new CompleteOrderResponseDto();

            return ResultRoot.Success(resultDto);
        }

        public async Task<ResultDto<CreateOrderResponseDto>> CreateAsync(CreateOrderRequestDto dto, CancellationToken cancellationToken = default)
        {
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
    }
}
