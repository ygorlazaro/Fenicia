using FluentValidation;

namespace Fenicia.Auth.Domains.Company.Data;

/// <summary>
/// Validator for company update requests
/// </summary>
public class CompanyUpdateRequestValidator : AbstractValidator<CompanyUpdateRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyUpdateRequestValidator"/> class
    /// </summary>
    public CompanyUpdateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

        RuleFor(x => x.Timezone)
            .NotEmpty().WithMessage("Timezone is required.")
            .MaximumLength(100).WithMessage("Timezone must not exceed 100 characters.");
    }
}
