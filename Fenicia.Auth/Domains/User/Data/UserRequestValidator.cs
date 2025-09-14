namespace Fenicia.Auth.Domains.User.Data;

using Common.Database.Requests;

using Company.Data;

using FluentValidation;

public class UserRequestValidator : AbstractValidator<UserRequest>
{
    public UserRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.").MaximumLength(maximumLength: 48).WithMessage("Password must not exceed 48 characters.");

        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(maximumLength: 32).WithMessage("Name must not exceed 32 characters.");

        RuleFor(x => x.Company).NotNull().WithMessage("Company is required.").SetValidator(new CompanyRequestValidator());
    }
}
