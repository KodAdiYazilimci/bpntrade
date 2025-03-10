namespace BpnTrade.Domain.Entities.Persistence
{
    public class OrderItemEntity : EntityBase
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual OrderEntity Order { get; set; }
    }
}
