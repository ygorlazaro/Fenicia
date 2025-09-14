namespace Fenicia.Auth.Domains.Company.Data;

using Common.Database.Requests;

using FluentValidation;

public class CompanyUpdateRequestValidator : AbstractValidator<CompanyUpdateRequest>
{
    public CompanyUpdateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(maximumLength: 50).WithMessage("Name must not exceed 50 characters.");

        RuleFor(x => x.Timezone).NotEmpty().WithMessage("Timezone is required.").MaximumLength(maximumLength: 100).WithMessage("Timezone must not exceed 100 characters.");
    }
}
