using BpnTrade.Domain.Dto;

namespace BpnTrade.Domain.Services
{
    public interface IOrderService
    {
        Task<ResultDto<OrderDto>> CrateAsync(OrderDto orderDto, CancellationToken cancellationToken = default);
    }
}
