using Fenicia.Common.Data.Requests.Auth;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.Auth;

public class CompanyValidation : AbstractValidator<CompanyRequest>
{
    public CompanyValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(50)
            .WithMessage("Name cannot exceed 50 characters");

        RuleFor(x => x.Cnpj)
            .NotEmpty()
            .WithMessage("CNPJ is required")
            .MaximumLength(14)
            .WithMessage("CNPJ cannot exceed 14 characters")
            .IsValidCNPJ()
            .WithMessage("CNPJ is invalid");
    }
}