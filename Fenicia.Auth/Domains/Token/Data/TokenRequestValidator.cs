namespace Fenicia.Auth.Domains.Token.Data;

using Common.Database.Requests;

using FluentValidation;

public class TokenRequestValidator : AbstractValidator<TokenRequest>
{
    public TokenRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.").MaximumLength(maximumLength: 48).WithMessage("Password must not exceed 48 characters.");

        RuleFor(x => x.Cnpj).NotEmpty().WithMessage("CNPJ is required.").MaximumLength(maximumLength: 14).WithMessage("CNPJ must not exceed 14 characters.");
    }
}
