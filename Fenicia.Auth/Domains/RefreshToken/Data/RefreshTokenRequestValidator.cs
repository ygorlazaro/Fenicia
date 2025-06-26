using FluentValidation;

namespace Fenicia.Auth.Domains.RefreshToken.Data;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("AccessToken is required.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("RefreshToken is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("CompanyId is required.");
    }
}
