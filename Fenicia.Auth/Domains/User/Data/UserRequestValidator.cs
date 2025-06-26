using Fenicia.Auth.Domains.Company.Data;

using FluentValidation;

namespace Fenicia.Auth.Domains.User.Data;

using Microsoft.Extensions.Logging;

/// <summary>
/// Validator for user registration requests
/// </summary>
public class UserRequestValidator : AbstractValidator<UserRequest>
{
    private readonly ILogger<UserRequestValidator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRequestValidator"/> class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    public UserRequestValidator(ILogger<UserRequestValidator> logger)
    {
        _logger = logger;
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
