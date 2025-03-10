namespace BpnTrade.Domain.Dto.Integration.Common
{
    public class UserOrderDto
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Status { get; set; }
        public DateTime CompletedAt { get; set; }
        public DateTime CancelledAt { get; set; }
    }
}
