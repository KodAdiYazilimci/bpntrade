namespace BpnTrade.Domain.Entities
{
    public class OrderItemEntity : EntityBase
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual OrderEntity Order { get; set; }
    }
}
