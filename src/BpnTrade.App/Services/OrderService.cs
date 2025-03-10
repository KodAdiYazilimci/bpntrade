using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BpnTrade.App.Services
{
    public class OrderService : IOrderService
    {
        public Task<ResultDto<OrderDto>> CrateAsync(OrderDto orderDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
