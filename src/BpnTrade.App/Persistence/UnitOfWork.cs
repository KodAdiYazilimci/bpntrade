using BpnTrade.Domain.Persistence;

using Microsoft.Extensions.DependencyInjection;

namespace BpnTrade.App.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWork(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TRepository GetRepository<TRepository>()
        {
            return _serviceProvider.GetRequiredService<TRepository>();
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            var dbContext = _serviceProvider.GetRequiredService<BpnContext>();

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
