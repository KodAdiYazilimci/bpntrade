using BpnTrade.Domain.Entities.Persistence;

namespace BpnTrade.Domain.Repositories
{
    public interface IRepositoryAsync<TEntity> where TEntity : EntityBase, new()
    {
        Task Create(TEntity entity, CancellationToken cancellationToken = default);
        Task Update(long id, TEntity entity, CancellationToken cancellationToken = default);
        Task Delete(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity?> GetAsync(long id, CancellationToken cancellationToken = default);
    }
}
