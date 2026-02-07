using Fenicia.Common.Data.Requests.Auth;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.Auth;

public class UserRequestValidation : AbstractValidator<UserRequest>
{
    public UserRequestValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is required");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password length must be at least 8 characters");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MinimumLength(3)
            .WithMessage("Name length must be at least 3 characters");

        RuleFor(x => x.Company)
            .NotEmpty()
            .WithMessage("Company is required");
    }
}