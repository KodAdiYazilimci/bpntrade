using BpnTrade.Domain.Entities.Persistence;
using BpnTrade.Domain.EntityTypeConfigurations;

using Microsoft.EntityFrameworkCore;

namespace BpnTrade.App.Persistence
{
    public class BpnContext : DbContext
    {
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderItemEntity> OrderItems { get; set; }

        public BpnContext(DbContextOptions<BpnContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<OrderEntity>(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration<OrderItemEntity>(new OrderItemEntityTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
