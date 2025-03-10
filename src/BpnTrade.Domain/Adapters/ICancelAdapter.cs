using BpnTrade.Domain.Dto;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BpnTrade.Domain.Adapters
{
    public interface ICancelAdapter
    {
        Task<ResultDto<CancelResponseDto>>
    }
}
