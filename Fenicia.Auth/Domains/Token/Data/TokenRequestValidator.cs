namespace Fenicia.Auth.Domains.Token.Data;

using FluentValidation;

/// <summary>
///     Validator for token request data.
///     Validates email format, password length, and CNPJ format.
/// </summary>
public class TokenRequestValidator : AbstractValidator<TokenRequest>
{
    private readonly ILogger<TokenRequestValidator> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TokenRequestValidator" /> class.
    ///     Sets up validation rules for TokenRequest properties.
    /// </summary>
    /// <param name="logger">Logger instance for validation failures</param>
    public TokenRequestValidator(ILogger<TokenRequestValidator> logger)
    {
        _logger = logger;
        RuleFor(x => x.Email).NotEmpty().WithMessage(errorMessage: "Email is required.").EmailAddress().WithMessage(errorMessage: "Email must be a valid email address.");

        RuleFor(x => x.Password).NotEmpty().WithMessage(errorMessage: "Password is required.").MaximumLength(maximumLength: 48).WithMessage(errorMessage: "Password must not exceed 48 characters.");

        RuleFor(x => x.Cnpj).NotEmpty().WithMessage(errorMessage: "CNPJ is required.").MaximumLength(maximumLength: 14).WithMessage(errorMessage: "CNPJ must not exceed 14 characters.");
    }
}
