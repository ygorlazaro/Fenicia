using Fenicia.Common.Data.Requests.Auth;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.Auth;

public class CompanyUpdateValidation : AbstractValidator<CompanyUpdateRequest>
{
    public CompanyUpdateValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(50)
            .WithMessage("Name cannot exceed 50 characters");

        RuleFor(x => x.Timezone)
            .NotEmpty()
            .WithMessage("Timezone is required")
            .Must(timezone => timezone != "UTC")
            .WithMessage("Timezone is invalid");
    }
}