using BpnTrade.App.Persistence;
using BpnTrade.Domain.Entities.Persistence;
using BpnTrade.Domain.Repositories.EF;

using Microsoft.EntityFrameworkCore;

namespace BpnTrade.App.Repositories.EF
{
    public class OrderItemRepository : RepositoryBaseAsync<OrderItemEntity>
    {
        private readonly BpnContext _dbContext;

        public OrderItemRepository(BpnContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task UpdateAsync(long id, OrderItemEntity entity, CancellationToken cancellationToken = default)
        {
            var existingEntity = await _dbContext.OrderItems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            existingEntity.UnitPrice = entity.UnitPrice;
            existingEntity.Quantity = entity.Quantity;
            existingEntity.ProductId = entity.ProductId;
            existingEntity.OrderId = entity.OrderId;
        }
    }
}
