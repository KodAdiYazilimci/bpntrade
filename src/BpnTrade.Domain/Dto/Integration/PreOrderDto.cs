using BpnTrade.Domain.Dto.Integration.Common;

namespace BpnTrade.Domain.Dto.Integration
{
    public class PreOrderRequestDto
    {
        public decimal Amount { get; set; }
        public string OrderId { get; set; }
    }

    public class PreOrderResponseDto : BpnResponseBase
    {
        public PreOrderResponseData Data { get; set; }
    }

    public class PreOrderResponseData
    {
        public UserOrderDto Order { get; set; }
        public UserBalanceDto UpdatedBalance { get; set; }
    }
}
