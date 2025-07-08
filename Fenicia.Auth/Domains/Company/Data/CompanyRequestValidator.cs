namespace Fenicia.Auth.Domains.Company.Data;

using FluentValidation;

public class CompanyRequestValidator : AbstractValidator<CompanyRequest>
{
    public CompanyRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(errorMessage: "Name is required.").MinimumLength(minimumLength: 2).WithMessage(errorMessage: "Name must be at least 2 characters.").MaximumLength(maximumLength: 50).WithMessage(errorMessage: "Name must not exceed 50 characters.").Matches(expression: @"^[a-zA-Z0-9\s\-\\. ]+$").WithMessage(errorMessage: "Name can only contain letters, numbers, spaces, hyphens and dots.");

        RuleFor(x => x.Cnpj).NotEmpty().WithMessage(errorMessage: "CNPJ is required.").Length(exactLength: 14).WithMessage(errorMessage: "CNPJ must be exactly 14 characters.").Matches(expression: @"^\d{14}$").WithMessage(errorMessage: "CNPJ must contain only numbers.");
    }
}
