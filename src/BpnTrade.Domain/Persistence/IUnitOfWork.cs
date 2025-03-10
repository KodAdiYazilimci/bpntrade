using BpnTrade.Domain.Dto;

namespace BpnTrade.Domain.Persistence
{
    public interface IUnitOfWork
    {
        TRepository GetRepository<TRepository>();
        Task<ResultDto> SaveAsync(CancellationToken cancellationToken = default);
    }
}
