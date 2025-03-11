namespace BpnTrade.Domain.Dto.Order
{
    public class CreateOrderRequestDto
    {
        public string Currency { get; set; }
        public List<CreateOrderItemRequestDto> OrderItems { get; set; }
        public string UserId { get; set; }
    }

    public class CreateOrderItemRequestDto
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
