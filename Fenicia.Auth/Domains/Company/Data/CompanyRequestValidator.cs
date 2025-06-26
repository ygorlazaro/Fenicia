using FluentValidation;

namespace Fenicia.Auth.Domains.Company.Data;

/// <summary>
/// Validator for company creation requests
/// </summary>
/// <remarks>
/// This validator ensures that company data meets the required format and constraints
/// </remarks>
public class CompanyRequestValidator : AbstractValidator<CompanyRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyRequestValidator"/> class
    /// </summary>
    /// <remarks>
    /// Sets up validation rules for company name and CNPJ
    /// </remarks>
    public CompanyRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.")
            .Matches(@"^[a-zA-Z0-9\s\-\.]+$").WithMessage("Name can only contain letters, numbers, spaces, hyphens and dots.");

        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("CNPJ is required.")
            .Length(14).WithMessage("CNPJ must be exactly 14 characters.")
            .Matches(@"^\d{14}$").WithMessage("CNPJ must contain only numbers.");
    }
}
