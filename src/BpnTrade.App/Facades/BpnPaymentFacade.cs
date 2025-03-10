using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Payment;
using BpnTrade.Domain.Facades;
using BpnTrade.Domain.Roots;

namespace BpnTrade.App.Facades
{
    public class BpnPaymentFacade : IPaymentFacade
    {
        private readonly IBalanceAdapter _balanceAdapter;
        private readonly IPreOrderAdapter _preOrderAdapter;
        private readonly ICompleteAdapter _completeAdapter;
        private readonly ICancelAdapter _cancelAdapter;

        public BpnPaymentFacade(
            IBalanceAdapter balanceAdapter,
            IPreOrderAdapter preOrderAdapter,
            ICompleteAdapter completeAdapter,
            ICancelAdapter cancelAdapter)
        {
            _balanceAdapter = balanceAdapter;
            _preOrderAdapter = preOrderAdapter;
            _completeAdapter = completeAdapter;
            _cancelAdapter = cancelAdapter;
        }

        public async Task<ResultDto<ProcessPaymentResponseDto>> ProcessPayment(ProcessPaymentRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            var balanceCheckResult = await _balanceAdapter.GetUserBalanceAsync(new Domain.Dto.Integration.BalanceRequestDto()
            {
                UserId = requestDto.UserId
            }, cancellationToken);

            if (!balanceCheckResult.IsSuccess)
            {
                return ResultRoot.Failure<ProcessPaymentResponseDto>(balanceCheckResult.Error);
            }

            if (balanceCheckResult.Data.Data.TotalBalance - balanceCheckResult.Data.Data.BlockedBalance <= 0)
            {
                return ResultRoot.Failure<ProcessPaymentResponseDto>(new ErrorDto("BLC001", "Yetersiz bakiye"));
            }

            return ResultRoot.Success(new ProcessPaymentResponseDto()
            {

            });
        }
    }
}
