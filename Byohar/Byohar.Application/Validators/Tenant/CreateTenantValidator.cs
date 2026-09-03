using Byohar.Application.Interfaces.Persistance;
using Byohar.Application.Requests.Tenant;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Byohar.Application.Validators.Tenant;

internal class CreateTenantValidator : AbstractValidator<CreateTenantRequest>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTenantValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        string namePattern = "^[A-Za-z]+$";

        // Rule for Email - Work Email Validation
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.")
            .Must(BeAWorkEmail).WithMessage("Only work emails are allowed."); //  work email validation

        // Rule for FirstName - Only alphabets allowed
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Matches(namePattern).WithMessage("First name must contain only alphabets."); // No digits allowed

        // Rule for LastName - Only alphabets allowed
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Matches(namePattern).WithMessage("Last name must contain only alphabets."); // No digits allowed

        // Rule for PhoneNumber
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\d+$").WithMessage("Phone number must contain only digits."); // E.164 format validation
    }

    // Custom validation method to ensure the email is a work email
    private bool BeAWorkEmail(string email)
    {
        // List of popular email domains to restrict (matching the frontend)
        var restrictedDomains = new List<string>
        {
            "gmail.com", "yahoo.com", "hotmail.com", "outlook.com", "icloud.com",
            "aol.com", "protonmail.com", "zoho.com", "gmx.com", "yandex.com"
        };

        // Extract domain from email
        var emailDomain = email.Split('@').LastOrDefault()?.ToLower();

        // Check if the domain is not in the restricted list
        return emailDomain != null && !restrictedDomains.Contains(emailDomain);
    }
}