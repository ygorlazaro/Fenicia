using Fenicia.Auth.Domains.Company.Logic;

using FluentValidation;

namespace Fenicia.Auth.Domains.User.Data;

public class UserRequestValidator : AbstractValidator<UserRequest>
{
    public UserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MaximumLength(48).WithMessage("Password must not exceed 48 characters.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(32).WithMessage("Name must not exceed 32 characters.");

        RuleFor(x => x.Company)
            .NotNull().WithMessage("Company is required.")
            .SetValidator(new CompanyRequestValidator());
    }
}
