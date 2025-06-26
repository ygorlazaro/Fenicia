using FluentValidation;

namespace Fenicia.Auth.Domains.RefreshToken.Data;

/// <summary>
/// Validator for refresh token requests to ensure all required fields are provided
/// </summary>
public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    private readonly ILogger<RefreshTokenRequestValidator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenRequestValidator"/> class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    public RefreshTokenRequestValidator(ILogger<RefreshTokenRequestValidator> logger)
    {
        _logger = logger;

        /// <summary>
        /// Validates that AccessToken is not empty
        /// </summary>
        RuleFor(x => x.AccessToken)
            .NotEmpty()
            .WithMessage("AccessToken is required.");

        /// <summary>
        /// Validates that RefreshToken is not empty
        /// </summary>
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("RefreshToken is required.");

        /// <summary>
        /// Validates that UserId is not empty
        /// </summary>
        RuleFor(x => x.UserId);

        /// <summary>
        /// Validates that CompanyId is not empty
        /// </summary>
        RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage("CompanyId is required.");
    }
}
