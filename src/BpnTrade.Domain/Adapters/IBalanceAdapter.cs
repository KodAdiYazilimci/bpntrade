using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;

namespace BpnTrade.Domain.Adapters
{
    public interface IBalanceAdapter
    {
        Task<ResultDto<BalanceResponseDto>> GetUserBalanceAsync(BalanceRequestDto requestDto, CancellationToken cancellationToken = default);
    }
}
