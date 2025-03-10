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

    public class BalanceResponseData
    {
        public string UserId { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal BlockedBalance { get; set; }
        public string Currency { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
