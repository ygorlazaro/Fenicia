using FluentValidation;

namespace Fenicia.Auth.Domains.Token.Logic;

public class TokenRequestValidator : AbstractValidator<TokenRequest>
{
    public TokenRequestValidator()
    {
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
