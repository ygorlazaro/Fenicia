using FluentValidation;

namespace Fenicia.Auth.Domains.ForgotPassword.Data;

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.");
    }
}
