namespace Fenicia.Auth.Domains.User.Data;

using Company.Data;

using FluentValidation;

public class UserRequestValidator : AbstractValidator<UserRequest>
{
    private readonly ILogger<UserRequestValidator> _logger;

    public UserRequestValidator(ILogger<UserRequestValidator> logger)
    {
        _logger = logger;
        RuleFor(x => x.Email).NotEmpty().WithMessage(errorMessage: "Email is required.").EmailAddress().WithMessage(errorMessage: "Email must be a valid email address.");

        RuleFor(x => x.Password).NotEmpty().WithMessage(errorMessage: "Password is required.").MaximumLength(maximumLength: 48).WithMessage(errorMessage: "Password must not exceed 48 characters.");

        RuleFor(x => x.Name).NotEmpty().WithMessage(errorMessage: "Name is required.").MaximumLength(maximumLength: 32).WithMessage(errorMessage: "Name must not exceed 32 characters.");

        RuleFor(x => x.Company).NotNull().WithMessage(errorMessage: "Company is required.").SetValidator(new CompanyRequestValidator());
    }
}
