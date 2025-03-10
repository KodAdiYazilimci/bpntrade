using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;

namespace BpnTrade.Domain.Adapters
{
    public interface ICancelAdapter
    {
        Task<ResultDto<CancelResponseDto>> CancelAsync(CancelRequestDto requestDto, CancellationToken cancellationToken = default);
    }
}
