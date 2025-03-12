using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;
using BpnTrade.Domain.Dto.Order;
using BpnTrade.Domain.Dto.Payment;
using BpnTrade.Domain.Entities;
using BpnTrade.Domain.Facades;
using BpnTrade.Domain.Persistence;
using BpnTrade.Domain.Repositories.EF;
using BpnTrade.Domain.Roots;

using Microsoft.EntityFrameworkCore;

using System.Threading;

namespace BpnTrade.App.Facades
{
    public class BpnPaymentFacade : IPaymentFacade
    {
        private readonly IBalanceAdapter _balanceAdapter;
        private readonly IPreOrderAdapter _preOrderAdapter;
        private readonly ICompleteAdapter _completeAdapter;
        private readonly ICancelAdapter _cancelAdapter;
        private readonly IProductAdapter _productAdapter;
        private readonly IUnitOfWork _unitOfWork;

        public BpnPaymentFacade(
            IBalanceAdapter balanceAdapter,
            IPreOrderAdapter preOrderAdapter,
            ICompleteAdapter completeAdapter,
            ICancelAdapter cancelAdapter,
            IProductAdapter productAdapter,
            IUnitOfWork unitOfWork)
        {
            _balanceAdapter = balanceAdapter;
            _preOrderAdapter = preOrderAdapter;
            _completeAdapter = completeAdapter;
            _cancelAdapter = cancelAdapter;
            _productAdapter = productAdapter;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<ProcessPaymentResponseDto>> ProcessPayment(ProcessPaymentRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            var validatedProducts = await ValidateProductsExists(requestDto, cancellationToken);

            if (!validatedProducts)
            {
                return ResultRoot.Failure<ProcessPaymentResponseDto>(new ErrorDto("PRD003", "Ürün stoğu veya fiyatı artık geçerli değil"));
            }

            var balanceCheckResult = await _balanceAdapter.GetUserBalanceAsync(new BalanceRequestDto()
            {
                UserId = requestDto.UserId
            }, cancellationToken);

            if (!balanceCheckResult.IsSuccess || !balanceCheckResult.Data.Success)
            {
                return ResultRoot.Failure<ProcessPaymentResponseDto>(balanceCheckResult.Error);
            }

            if (balanceCheckResult.Data.Data.TotalBalance - balanceCheckResult.Data.Data.BlockedBalance < requestDto.Amount)
            {
                return ResultRoot.Failure<ProcessPaymentResponseDto>(new ErrorDto("BLC001", "Yetersiz bakiye"));
            }

            // ön ödeme sonrası sepet tutarı değişmişse
            if (balanceCheckResult.Data.Data.BlockedBalance != requestDto.Amount)
            {
                // önceki ön ödemeyi iptal et:
                var cancelResult = await _cancelAdapter.CancelAsync(new CancelRequestDto()
                {
                    OrderId = requestDto.OrderId
                }, cancellationToken);

                if (!cancelResult.IsSuccess || !cancelResult.Data.Success)
                {
                    return ResultRoot.Failure<ProcessPaymentResponseDto>(cancelResult.Error);
                }

                // yenisini oluştur:
                var preOrderResult = await _preOrderAdapter.PreOrderAsync(new PreOrderRequestDto()
                {
                    OrderId = requestDto.OrderId,
                    Amount = requestDto.Amount
                }, cancellationToken);

                if (!preOrderResult.IsSuccess || !preOrderResult.Data.Success)
                {
                    return ResultRoot.Failure<ProcessPaymentResponseDto>(preOrderResult.Error);
                }
            }

            var completeResult = await _completeAdapter.CompleteAsync(new CompleteRequestDto()
            {
                OrderId = requestDto.OrderId
            }, cancellationToken);

            if (!completeResult.IsSuccess)
            {
                return ResultRoot.Failure<ProcessPaymentResponseDto>(completeResult.Error);
            }

            return ResultRoot.Success(new ProcessPaymentResponseDto()
            {

            });
        }
        private async Task<bool> ValidateProductsExists(ProcessPaymentRequestDto dto, CancellationToken cancellationToken = default)
        {
            var products = await _productAdapter.GetProductsAsync(cancellationToken);

            if (products?.IsSuccess ?? false && (products?.Data?.Success ?? false) && (products?.Data?.Data?.Any() ?? false))
            {
                var orderItems =
                    await
                    _unitOfWork
                    .Context
                    .Set<OrderItemEntity>()
                    .AsNoTracking()
                    .Where(x => x.OrderId == dto.OrderId && x.DeleteDate == null)
                    .Select(x => new { x.ProductId, x.UnitPrice, x.Quantity })
                    .ToListAsync(cancellationToken);


                foreach (var orderItem in orderItems)
                {
                    var product = products.Data.Data.Where(x => x.Id == orderItem.ProductId).FirstOrDefault();

                    if (product != null || product.Price != orderItem.UnitPrice && product.Stock < orderItem.Quantity)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
