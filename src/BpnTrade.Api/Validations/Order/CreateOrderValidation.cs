using BpnTrade.Domain.Dto.Order;

using FluentValidation;

namespace BpnTrade.Api.Validations.Order
{
    public class CreateOrderValidationRule : AbstractValidator<CreateOrderRequestDto>
    {
        public CreateOrderValidationRule()
        {
            RuleFor(x => x.OrderItems).NotNull().WithErrorCode("VAL001").WithMessage("Sipariş öğeleri boş geçilemez");
            RuleFor(x => x.OrderItems).Empty().WithErrorCode("VAL002").WithMessage("Sipariş öğeleri boş geçilemez");
        }
    }

    public class CreateOrderValidator : ValidatorBase<CreateOrderRequestDto, CreateOrderValidationRule>
    {
        public static CreateOrderValidator Instance
        {
            get { return new CreateOrderValidator(); }
        }
    }
}
