using FluentValidation;

namespace Fenicia.Auth.Domains.ForgotPassword.Data;

/// <summary>
/// Validator class for the ForgotPasswordRequest model.
/// Implements comprehensive validation rules for the forgot password request process.
/// </summary>
/// <remarks>
/// This validator ensures that the email provided meets all required criteria
/// including format validation and length restrictions.
/// </remarks>
public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    private readonly ILogger<ForgotPasswordRequestValidator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForgotPasswordRequestValidator"/> class.
    /// Sets up validation rules for the email field with comprehensive checks.
    /// </summary>
    /// <param name="logger">The logger instance for validation tracking.</param>
    public ForgotPasswordRequestValidator(ILogger<ForgotPasswordRequestValidator> logger)
    {
        _logger = logger;

        try
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters.")
                .Must(email => email?.Contains("@") == true).WithMessage("Email must contain @ symbol.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while setting up validation rules");
            throw;
        }
    }
}
