using BpnTrade.Domain.Entities;

namespace BpnTrade.Domain.Repositories
{
    public interface IRepository<TEntity> where TEntity : EntityBase, new()
    {
        void Create(TEntity entity);
        void Update(long id, TEntity entity);
        void Delete(TEntity entity);
        TEntity? Get(long id);
    }
}
