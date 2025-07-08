namespace Fenicia.Auth.Domains.Company.Data;

using FluentValidation;

public class CompanyUpdateRequestValidator : AbstractValidator<CompanyUpdateRequest>
{
    public CompanyUpdateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(errorMessage: "Name is required.").MaximumLength(maximumLength: 50).WithMessage(errorMessage: "Name must not exceed 50 characters.");

        RuleFor(x => x.Timezone).NotEmpty().WithMessage(errorMessage: "Timezone is required.").MaximumLength(maximumLength: 100).WithMessage(errorMessage: "Timezone must not exceed 100 characters.");
    }
}
