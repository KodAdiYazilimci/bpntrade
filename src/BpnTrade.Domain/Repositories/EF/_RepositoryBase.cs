using Microsoft.EntityFrameworkCore;
using BpnTrade.Domain.Entities;

namespace BpnTrade.Domain.Repositories.EF
{
    public abstract class RepositoryBaseAsync<TEntity> where TEntity : EntityBase, new()
    {
        private readonly DbContext _dbContext;
        protected RepositoryBaseAsync(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<TEntity>().Add(entity);

            return Task.CompletedTask;
        }

        public abstract Task UpdateAsync(long id, TEntity entity, CancellationToken cancellationToken = default);

        public Task Delete(TEntity entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<TEntity>().Remove(entity);

            return Task.CompletedTask;
        }

        public Task<TEntity?> GetAsync(long id, CancellationToken cancellationToken = default)
        {
            return GetQueryable().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return _dbContext.Set<TEntity>().Where(x => x.DeleteDate == null).AsQueryable();
        }
    }
}
