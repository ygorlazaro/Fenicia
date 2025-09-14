namespace Fenicia.Auth.Domains.Company.Data;

using Common.Database.Requests;

using FluentValidation;

public class CompanyRequestValidator : AbstractValidator<CompanyRequest>
{
    public CompanyRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MinimumLength(minimumLength: 2).WithMessage("Name must be at least 2 characters.").MaximumLength(maximumLength: 50).WithMessage("Name must not exceed 50 characters.").Matches(@"^[a-zA-Z0-9\s\-\\. ]+$").WithMessage("Name can only contain letters, numbers, spaces, hyphens and dots.");

        RuleFor(x => x.Cnpj).NotEmpty().WithMessage("CNPJ is required.").Length(exactLength: 14).WithMessage("CNPJ must be exactly 14 characters.").Matches(@"^\d{14}$").WithMessage("CNPJ must contain only numbers.");
    }
}
