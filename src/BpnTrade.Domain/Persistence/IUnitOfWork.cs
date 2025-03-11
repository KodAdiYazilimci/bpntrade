using BpnTrade.Domain.Dto;

using Microsoft.EntityFrameworkCore;

namespace BpnTrade.Domain.Persistence
{
    public interface IUnitOfWork
    {
        DbContext Context { get; }
        TRepository GetRepository<TRepository>();
        Task<ResultDto> SaveAsync(CancellationToken cancellationToken = default);
    }
}
