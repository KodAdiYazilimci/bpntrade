using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Entities.Integration;

namespace BpnTrade.Domain.Adapters
{
    public interface IBalanceAdapter
    {
        Task<ResultDto<BalanceEntity>> GetUserBalanceAsync(string userId, CancellationToken cancellationToken = default);
    }
}
