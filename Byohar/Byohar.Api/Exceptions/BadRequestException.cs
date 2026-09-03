using FluentValidation.Results;

namespace Byohar.Api.Exceptions;

public class BadRequestException : Exception
{
    public IDictionary<string, string[]>? ValidationErrors { get; set; }
    public BadRequestException(string message) : base(message)
    {

    }

    public BadRequestException(string message, ValidationResult validationResult) : base(message)
    {
        ValidationErrors = validationResult.ToDictionary();
    }

}
