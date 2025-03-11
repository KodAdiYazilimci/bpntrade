using BpnTrade.Domain.Entities;

namespace BpnTrade.Domain.Repositories.EF
{
    public interface IOrderItemRepository : IRepositoryAsync<OrderItemEntity>
    {
        Task CreateAsync(OrderItemEntity orderItem, CancellationToken cancellationToken = default);
    }
}
