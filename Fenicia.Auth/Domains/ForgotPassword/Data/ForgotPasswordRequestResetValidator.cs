namespace Fenicia.Auth.Domains.ForgotPassword.Data;

using FluentValidation;

/// <summary>
///     Validates password reset requests ensuring all required fields are properly formatted
/// </summary>
public class ForgotPasswordRequestResetValidator : AbstractValidator<ForgotPasswordRequestReset>
{
    private readonly ILogger<ForgotPasswordRequestResetValidator> _logger;

    /// <summary>
    ///     Initializes a new instance of the validator with validation rules for email, code and password
    /// </summary>
    /// <param name="logger">Logger instance for validation events</param>
    public ForgotPasswordRequestResetValidator(ILogger<ForgotPasswordRequestResetValidator> logger)
    {
        _logger = logger;

        RuleFor(x => x.Email).NotEmpty().WithMessage(errorMessage: "Email is required.").EmailAddress().WithMessage(errorMessage: "Invalid email format.").Must(email =>
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                return true;
            }

            _logger.LogWarning(message: "Empty email provided in password reset request");
            return false;
        });

        RuleFor(x => x.Code).NotEmpty().WithMessage(errorMessage: "Code is required.").MinimumLength(minimumLength: 6).WithMessage(errorMessage: "Code must be at least 6 characters.").Must(code =>
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                return true;
            }

            _logger.LogWarning(message: "Empty verification code provided");
            return false;
        });

        RuleFor(x => x.Password).NotEmpty().WithMessage(errorMessage: "Password is required.").MaximumLength(maximumLength: 48).WithMessage(errorMessage: "Password must not exceed 48 characters.").MinimumLength(minimumLength: 8).WithMessage(errorMessage: "Password must be at least 8 characters.").Must(password =>
        {
            if (!string.IsNullOrWhiteSpace(password))
            {
                return true;
            }

            _logger.LogWarning(message: "Empty password provided in reset request");
            return false;
        });
    }
}
