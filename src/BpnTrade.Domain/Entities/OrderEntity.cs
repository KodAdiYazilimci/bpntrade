﻿namespace BpnTrade.Domain.Entities
{
    public class OrderEntity : EntityBase
    {
        public int CustomerId { get; set; }
        public DateTime? PaymentDate { get; set; }

        public virtual ICollection<OrderItemEntity> OrderItems { get; set; }
    }
}
