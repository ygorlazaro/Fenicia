using FluentValidation;

namespace Fenicia.Auth.Domains.ForgotPassword.Data;

/// <summary>
/// Validates password reset requests ensuring all required fields are properly formatted
/// </summary>
public class ForgotPasswordRequestResetValidator : AbstractValidator<ForgotPasswordRequestReset>
{
    private readonly ILogger<ForgotPasswordRequestResetValidator> _logger;

    /// <summary>
    /// Initializes a new instance of the validator with validation rules for email, code and password
    /// </summary>
    /// <param name="logger">Logger instance for validation events</param>
    public ForgotPasswordRequestResetValidator(ILogger<ForgotPasswordRequestResetValidator> logger)
    {
        _logger = logger;

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .Must(email =>
            {
                if (!string.IsNullOrWhiteSpace(email))
                {
                    return true;
                }

                _logger.LogWarning("Empty email provided in password reset request");
                return false;
            });

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .MinimumLength(6).WithMessage("Code must be at least 6 characters.")
            .Must(code =>
            {
                if (!string.IsNullOrWhiteSpace(code))
                {
                    return true;
                }

                _logger.LogWarning("Empty verification code provided");
                return false;
            });

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MaximumLength(48).WithMessage("Password must not exceed 48 characters.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Must(password =>
            {
                if (!string.IsNullOrWhiteSpace(password))
                {
                    return true;
                }

                _logger.LogWarning("Empty password provided in reset request");
                return false;
            });
    }
}
