using FluentValidation;

namespace Fenicia.Auth.Domains.ForgotPassword.AddForgotPassword;

public sealed class AddForgotPasswordValidator : AbstractValidator<AddForgotPasswordCommand>
{
    public AddForgotPasswordValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}