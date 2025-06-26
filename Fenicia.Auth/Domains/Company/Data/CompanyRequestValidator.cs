namespace Fenicia.Auth.Domains.Company.Data;

using FluentValidation;

/// <summary>
///     Validator for company creation requests
/// </summary>
/// <remarks>
///     This validator ensures that company data meets the required format and constraints
/// </remarks>
public class CompanyRequestValidator : AbstractValidator<CompanyRequest>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CompanyRequestValidator" /> class
    /// </summary>
    /// <remarks>
    ///     Sets up validation rules for company name and CNPJ
    /// </remarks>
    public CompanyRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(errorMessage: "Name is required.").MinimumLength(minimumLength: 2).WithMessage(errorMessage: "Name must be at least 2 characters.").MaximumLength(maximumLength: 50).WithMessage(errorMessage: "Name must not exceed 50 characters.").Matches(expression: @"^[a-zA-Z0-9\s\-\.]+$").WithMessage(errorMessage: "Name can only contain letters, numbers, spaces, hyphens and dots.");

        RuleFor(x => x.Cnpj).NotEmpty().WithMessage(errorMessage: "CNPJ is required.").Length(exactLength: 14).WithMessage(errorMessage: "CNPJ must be exactly 14 characters.").Matches(expression: @"^\d{14}$").WithMessage(errorMessage: "CNPJ must contain only numbers.");
    }
}
