using BpnTrade.Domain.Entities.Persistence;

namespace BpnTrade.Domain.Repositories.EF
{
    public interface IOrderRepository : IRepositoryAsync<OrderEntity>
    {
        Task CreateAsync(OrderEntity order, CancellationToken cancellationToken = default);
    }
}
