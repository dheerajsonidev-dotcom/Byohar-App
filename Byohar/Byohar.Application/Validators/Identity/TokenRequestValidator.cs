using Byohar.Application.Requests.Identity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Byohar.Application.Validators.Identity
{
    public class TokenRequestValidator : AbstractValidator<TokenRequest>
    {
        public TokenRequestValidator(IStringLocalizer<TokenRequestValidator> localizer)
        {
            RuleFor(request => request.UserName)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Email is required"]);
            RuleFor(request => request.Password)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Password is required!"]);
        }
    }
}
