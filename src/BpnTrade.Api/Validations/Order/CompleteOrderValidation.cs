using BpnTrade.Domain.Dto.Order;

using FluentValidation;

namespace BpnTrade.Api.Validations.Order
{
    public class CompleteOrderValidationRule : AbstractValidator<CompleteOrderRequestDto>
    {
        public CompleteOrderValidationRule()
        {
            RuleFor(x => x.OrderId).GreaterThan(0).WithErrorCode("VAL003").WithMessage("Geçersiz sipariş numarası");
        }
    }

    public class CompleteOrderValidator : ValidatorBase<CompleteOrderRequestDto, CompleteOrderValidationRule>
    {
        public static CompleteOrderValidator Instance
        {
            get { return new CompleteOrderValidator(); }
        }
    }
}
