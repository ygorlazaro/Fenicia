namespace Fenicia.Auth.Domains.ForgotPassword.Data;

using FluentValidation;

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    private readonly ILogger<ForgotPasswordRequestValidator> _logger;

    public ForgotPasswordRequestValidator(ILogger<ForgotPasswordRequestValidator> logger)
    {
        _logger = logger;

        try
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(errorMessage: "Email is required.").EmailAddress().WithMessage(errorMessage: "Invalid email format.").MaximumLength(maximumLength: 256).WithMessage(errorMessage: "Email must not exceed 256 characters.").Must(email => email?.Contains(value: "@") == true).WithMessage(errorMessage: "Email must contain @ symbol.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "Error occurred while setting up validation rules");
            throw;
        }
    }
}
