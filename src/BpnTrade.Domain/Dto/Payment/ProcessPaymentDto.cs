namespace BpnTrade.Domain.Dto.Payment
{
    public class ProcessPaymentRequestDto
    {
        public string UserId { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }

    public class ProcessPaymentResponseDto
    {

    }
}
