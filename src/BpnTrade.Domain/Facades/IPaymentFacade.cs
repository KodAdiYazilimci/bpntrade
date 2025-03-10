using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Payment;

namespace BpnTrade.Domain.Facades
{
    public interface IPaymentFacade
    {
        Task<ResultDto<ProcessPaymentResponseDto>> ProcessPayment(ProcessPaymentRequestDto requestDto, CancellationToken cancellationToken = default);
    }
}
