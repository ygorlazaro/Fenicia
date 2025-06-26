using FluentValidation;

namespace Fenicia.Auth.Domains.Token.Data;

/// <summary>
/// Validator for token request data.
/// Validates email format, password length, and CNPJ format.
/// </summary>
public class TokenRequestValidator : AbstractValidator<TokenRequest>
{
    private readonly ILogger<TokenRequestValidator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenRequestValidator"/> class.
    /// Sets up validation rules for TokenRequest properties.
    /// </summary>
    /// <param name="logger">Logger instance for validation failures</param>
    public TokenRequestValidator(ILogger<TokenRequestValidator> logger)
    {
        _logger = logger;
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MaximumLength(48).WithMessage("Password must not exceed 48 characters.");

        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("CNPJ is required.")
            .MaximumLength(14).WithMessage("CNPJ must not exceed 14 characters.");
    }
}
