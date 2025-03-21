﻿using BpnTrade.Domain.Dto.Integration.Common;

namespace BpnTrade.Domain.Dto.Integration
{
    public class CompleteRequestDto
    {
        public string OrderId { get; set; }
    }

    public class CompleteResponseDto : BpnResponseBase
    {
        public CompleteResponseData Data { get; set; }
    }

    public class CompleteResponseData
    {
        public UserOrderDto Order { get; set; }
        public UserBalanceDto UpdatedBalance { get; set; }
    }
}
