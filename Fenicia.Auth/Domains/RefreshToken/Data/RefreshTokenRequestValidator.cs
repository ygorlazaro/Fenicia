namespace Fenicia.Auth.Domains.RefreshToken.Data;

using FluentValidation;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty().WithMessage("AccessToken is required.");
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("RefreshToken is required.");
        RuleFor(x => x.UserId);
        RuleFor(x => x.CompanyId).NotEmpty().WithMessage("CompanyId is required.");
    }
}
