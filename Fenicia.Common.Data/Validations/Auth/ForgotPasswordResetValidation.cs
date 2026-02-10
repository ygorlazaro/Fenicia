using Fenicia.Common.Data.Requests.Auth;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.Auth;

public class ForgotPasswordResetValidation : AbstractValidator<ForgotPasswordResetRequest>
{
    public ForgotPasswordResetValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is invalid");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required")
            .Length(6)
            .WithMessage("Code is invalid");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");

    }
}
