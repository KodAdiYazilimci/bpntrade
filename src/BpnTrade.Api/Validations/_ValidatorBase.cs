using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Roots;

using FluentValidation;

namespace BpnTrade.Api.Validations
{
    public class ValidatorBase<TRequestDto, TRule> where TRule : AbstractValidator<TRequestDto>, new()
    {
        public virtual async Task<ResultDto> ValidateAsync(TRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            var validationRule = Activator.CreateInstance<TRule>();

            var validationResult = await validationRule.ValidateAsync(requestDto, cancellationToken);

            return
                validationResult.IsValid
                ?
                ResultRoot.Success()
                :
                ResultRoot.Failure(new ErrorDto(validationResult.Errors.First().ErrorCode, validationResult.Errors.First().ErrorMessage));
        }
    }
}
