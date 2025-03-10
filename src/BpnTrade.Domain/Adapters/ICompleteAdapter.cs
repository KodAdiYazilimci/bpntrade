using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;

namespace BpnTrade.Domain.Adapters
{
    public interface ICompleteAdapter
    {
        Task<ResultDto<CompleteResponseDto>> CompleteAsync(CompleteRequestDto requestDto, CancellationToken cancellationToken = default);
    }
}
