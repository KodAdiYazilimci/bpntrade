using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;

namespace BpnTrade.Domain.Adapters
{
    public interface IPreOrderAdapter
    {
        Task<ResultDto<PreOrderResponseDto>> PreOrderAsync(PreOrderRequestDto requestDto, CancellationToken cancellationToken = default);
    }
}
