namespace BpnTrade.Domain.Persistence
{
    public interface IUnitOfWork
    {
        TRepository GetRepository<TRepository>();
        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}
