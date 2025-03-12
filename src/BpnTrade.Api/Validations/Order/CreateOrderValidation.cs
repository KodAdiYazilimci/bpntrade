using BpnTrade.Domain.Dto.Order;

using FluentValidation;

namespace BpnTrade.Api.Validations.Order
{
    public class CreateOrderValidationRule : AbstractValidator<CreateOrderRequestDto>
    {
        public CreateOrderValidationRule()
        {
            RuleFor(x => x.OrderItems).NotNull().WithErrorCode("VAL001").WithMessage("Sipariş öğeleri boş geçilemez");
            RuleFor(x => x.OrderItems).NotEmpty().WithErrorCode("VAL002").WithMessage("Sipariş öğeleri boş geçilemez");
        }
    }

    public class CreateOrderValidator : ValidatorBase<CreateOrderRequestDto, CreateOrderValidationRule>
    {
        private static readonly object Lock = new object();
        private static CreateOrderValidator instance = null;
        public static CreateOrderValidator Instance
        {
            get
            {
                lock (Lock)
                {
                    if (instance == null)
                    {
                        instance = new CreateOrderValidator();
                    }
                }

                return instance;
            }
        }
    }
}
