namespace BpnTrade.Domain.Dto.Order
{
    public class CreateOrderRequestDto
    {
        public int CustomerId { get; set; }
        public List<CreateOrderItemRequestDto> OrderItems { get; set; }
    }

    public class CreateOrderItemRequestDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
