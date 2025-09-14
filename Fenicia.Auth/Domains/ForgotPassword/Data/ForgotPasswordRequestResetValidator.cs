namespace Fenicia.Auth.Domains.ForgotPassword.Data;

using Common.Database.Requests;

using FluentValidation;

public class ForgotPasswordRequestResetValidator : AbstractValidator<ForgotPasswordRequestReset>
{
    public ForgotPasswordRequestResetValidator(ILogger<ForgotPasswordRequestResetValidator> logger)
    {
        var logger1 = logger;

        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Invalid email format.").Must(email =>
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                return true;
            }

            logger1.LogWarning("Empty email provided in password reset request");
            return false;
        });

        RuleFor(x => x.Code).NotEmpty().WithMessage("Code is required.").MinimumLength(minimumLength: 6).WithMessage("Code must be at least 6 characters.").Must(code =>
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                return true;
            }

            logger1.LogWarning("Empty verification code provided");
            return false;
        });

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.").MaximumLength(maximumLength: 48).WithMessage("Password must not exceed 48 characters.").MinimumLength(minimumLength: 8).WithMessage("Password must be at least 8 characters.").Must(password =>
        {
            if (!string.IsNullOrWhiteSpace(password))
            {
                return true;
            }

            logger1.LogWarning("Empty password provided in reset request");
            return false;
        });
    }
}
