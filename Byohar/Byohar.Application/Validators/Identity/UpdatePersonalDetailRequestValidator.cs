using Byohar.Application.Requests.Identity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Byohar.Application.Validators.Identity
{
    public class UpdatePersonalDetailRequestValidator : AbstractValidator<UpdatePersonalDetailRequest>
    {
        public UpdatePersonalDetailRequestValidator(IStringLocalizer<UpdateProfileRequestValidator> localizer)
        {
            RuleFor(request => request.FirstName)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["First Name is required"]);
            RuleFor(request => request.LastName)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Last Name is required"]);
            RuleFor(request => request.Email)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Email is required"]);
            RuleFor(request => request.PhoneNumber)
                .Must(x => string.IsNullOrWhiteSpace(x) || x.Length == 10).WithMessage(x => localizer["Phone number must consist of 10 numbers"]);
        }
    }
}