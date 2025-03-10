using BpnTrade.App.Persistence;
using BpnTrade.Domain.Entities;
using BpnTrade.Domain.Repositories.EF;

using Microsoft.EntityFrameworkCore;

namespace BpnTrade.App.Repositories.EF
{
    public class OrderRepository : RepositoryBaseAsync<OrderEntity>
    {
        private readonly BpnContext _dbContext;

        public OrderRepository(BpnContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task UpdateAsync(long id, OrderEntity entity, CancellationToken cancellationToken = default)
        {
            var existingEntity = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            existingEntity.CustomerId = entity.CustomerId;
        }
    }
}
