namespace BpnTrade.Domain.Dto.Integration
{
    public class CompleteRequestDto
    {
        public string OrderId { get; set; }
    }

    public class CompleteResponseDto : BpnResponseBase
    {
        public CompleteResponseData Data { get; set; }
    }

    public class CompleteResponseData
    {
        public CompleteResponseOrder Order { get; set; }
        public CompleteResponseBalance UpdatedBalance { get; set; }
    }

    public class CompleteResponseOrder
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Status { get; set; }
        public DateTime CompletedAt { get; set; }
        public DateTime CancelledAt { get; set; }
    }

    public class CompleteResponseBalance
    {
        public string UserId { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal BlockedBalance { get; set; }
        public string Currency { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
