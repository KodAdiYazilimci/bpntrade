﻿using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Order;

namespace BpnTrade.Domain.Services
{
    public interface IOrderService
    {
        Task<ResultDto<CreateOrderResponseDto>> CreateAsync(CreateOrderRequestDto dto, CancellationToken cancellationToken = default);
        Task<ResultDto<CompleteOrderResponseDto>> CompleteOrderAsync(CompleteOrderRequestDto dto, CancellationToken cancellationToken = default);
    }
}
