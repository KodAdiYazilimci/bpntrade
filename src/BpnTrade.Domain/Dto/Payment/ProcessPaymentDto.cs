namespace BpnTrade.Domain.Dto.Payment
{
    public class ProcessPaymentRequestDto
    {
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
    }

    public class ProcessPaymentResponseDto
    {

    }
}
