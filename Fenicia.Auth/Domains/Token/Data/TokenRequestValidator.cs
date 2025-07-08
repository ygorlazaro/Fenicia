namespace Fenicia.Auth.Domains.Token.Data;

using FluentValidation;

public class TokenRequestValidator : AbstractValidator<TokenRequest>
{
    private readonly ILogger<TokenRequestValidator> _logger;

    public TokenRequestValidator(ILogger<TokenRequestValidator> logger)
    {
        _logger = logger;
        RuleFor(x => x.Email).NotEmpty().WithMessage(errorMessage: "Email is required.").EmailAddress().WithMessage(errorMessage: "Email must be a valid email address.");

        RuleFor(x => x.Password).NotEmpty().WithMessage(errorMessage: "Password is required.").MaximumLength(maximumLength: 48).WithMessage(errorMessage: "Password must not exceed 48 characters.");

        RuleFor(x => x.Cnpj).NotEmpty().WithMessage(errorMessage: "CNPJ is required.").MaximumLength(maximumLength: 14).WithMessage(errorMessage: "CNPJ must not exceed 14 characters.");
    }
}
