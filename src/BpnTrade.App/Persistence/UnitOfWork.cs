using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Persistence;
using BpnTrade.Domain.Roots;

using Microsoft.EntityFrameworkCore;
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

        public DbContext Context
        {
            get
            {
                return _serviceProvider.GetRequiredService<BpnContext>();
            }
        }

        public TRepository GetRepository<TRepository>()
        {
            return _serviceProvider.GetRequiredService<TRepository>();
        }

        public async Task<ResultDto> SaveAsync(CancellationToken cancellationToken = default)
        {
            var dbContext = _serviceProvider.GetRequiredService<BpnContext>();

            using (var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    await dbContext.SaveChangesAsync(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);

                    return ResultRoot.Success();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    return ResultRoot.Failure(new ErrorDto("DB", $"{ex.Message} {ex.InnerException?.Message}"));
                }
            }
        }
    }
}
