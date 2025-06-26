namespace Fenicia.Auth.Domains.Company.Data;

using FluentValidation;

/// <summary>
///     Validator for company update requests
/// </summary>
public class CompanyUpdateRequestValidator : AbstractValidator<CompanyUpdateRequest>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CompanyUpdateRequestValidator" /> class
    /// </summary>
    public CompanyUpdateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(errorMessage: "Name is required.").MaximumLength(maximumLength: 50).WithMessage(errorMessage: "Name must not exceed 50 characters.");

        RuleFor(x => x.Timezone).NotEmpty().WithMessage(errorMessage: "Timezone is required.").MaximumLength(maximumLength: 100).WithMessage(errorMessage: "Timezone must not exceed 100 characters.");
    }
}
