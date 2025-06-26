using FluentValidation;

namespace Fenicia.Auth.Domains.ForgotPassword.Data;

public class ForgotPasswordRequestResetValidator : AbstractValidator<ForgotPasswordRequestReset>
{
    public ForgotPasswordRequestResetValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MaximumLength(48).WithMessage("Password must not exceed 48 characters.");
    }
}
