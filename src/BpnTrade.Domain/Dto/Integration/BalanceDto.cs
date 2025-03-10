using BpnTrade.Domain.Dto.Integration.Common;

namespace BpnTrade.Domain.Dto.Integration
{
    public class BalanceRequestDto
    {
        public string UserId { get; set; }
    }

    public class BalanceResponseDto : BpnResponseBase
    {
        public BalanceResponseData Data { get; set; }
    }

    public class BalanceResponseData : UserBalanceDto
    {

    }
}
