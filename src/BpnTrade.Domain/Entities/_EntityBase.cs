﻿namespace BpnTrade.Domain.Entities
{
    public abstract class EntityBase
    {
        public int Id { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}
