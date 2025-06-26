namespace Fenicia.Auth.Domains.ForgotPassword.Data;

using FluentValidation;

/// <summary>
///     Validator class for the ForgotPasswordRequest model.
///     Implements comprehensive validation rules for the forgot password request process.
/// </summary>
/// <remarks>
///     This validator ensures that the email provided meets all required criteria
///     including format validation and length restrictions.
/// </remarks>
public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    private readonly ILogger<ForgotPasswordRequestValidator> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ForgotPasswordRequestValidator" /> class.
    ///     Sets up validation rules for the email field with comprehensive checks.
    /// </summary>
    /// <param name="logger">The logger instance for validation tracking.</param>
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
