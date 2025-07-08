namespace Fenicia.Auth.Domains.RefreshToken.Data;

using FluentValidation;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    private readonly ILogger<RefreshTokenRequestValidator> _logger;

    public RefreshTokenRequestValidator(ILogger<RefreshTokenRequestValidator> logger)
    {
        _logger = logger;

        RuleFor(x => x.AccessToken).NotEmpty().WithMessage(errorMessage: "AccessToken is required.");
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage(errorMessage: "RefreshToken is required.");
        RuleFor(x => x.UserId);
        RuleFor(x => x.CompanyId).NotEmpty().WithMessage(errorMessage: "CompanyId is required.");
    }
}
