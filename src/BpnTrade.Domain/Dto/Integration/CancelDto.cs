using BpnTrade.Domain.Dto.Integration.Common;

namespace BpnTrade.Domain.Dto.Integration
{
    public class CancelRequestDto
    {
        public string OrderId { get; set; }
    }

    public class CancelResponseDto : BpnResponseBase
    {
        public CancelResponseData Data { get; set; }
    }

    public class CancelResponseData
    {
        public UserOrderDto Order { get; set; }
        public UserBalanceDto UpdatedBalance { get; set; }
    }
}
