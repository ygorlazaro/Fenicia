namespace Fenicia.Auth.Domains.ForgotPassword.Data;

using Common.Database.Requests;

using FluentValidation;

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator(ILogger<ForgotPasswordRequestValidator> logger)
    {
        try
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Invalid email format.").MaximumLength(maximumLength: 256).WithMessage("Email must not exceed 256 characters.").Must(email => email?.Contains("@") == true).WithMessage("Email must contain @ symbol.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while setting up validation rules");
            throw;
        }
    }
}
