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
        public PreOrderResponseOrder Order { get; set; }
        public PreOrderResponseUpdatedBalance UpdatedBalance { get; set; }
    }

    public class PreOrderResponseOrder
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Status { get; set; }
        public DateTime CompletedAt { get; set; }
        public DateTime CancelledAt { get; set; }
    }

    public class PreOrderResponseUpdatedBalance
    {
        public string UserId { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal BlockedBalance { get; set; }
        public string Currency { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
