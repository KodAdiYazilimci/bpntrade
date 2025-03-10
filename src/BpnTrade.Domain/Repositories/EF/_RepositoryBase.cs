using Microsoft.EntityFrameworkCore;

using BpnTrade.Domain.Entities;

namespace BpnTrade.Domain.Repositories.EF
{
    public abstract class RepositoryBase<TEntity> where TEntity : EntityBase, new()
    {
        private readonly DbContext _dbContext;
        protected RepositoryBase(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }

        public abstract void Update(long id, TEntity entity);

        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public TEntity? Get(long id)
        {
            return _dbContext.Set<TEntity>().FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return _dbContext.Set<TEntity>().AsQueryable();
        }
    }
}
