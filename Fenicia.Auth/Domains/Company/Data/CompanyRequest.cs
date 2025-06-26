using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Company.Data;

/// <summary>
/// Represents a request model for company creation or modification.
/// </summary>
/// <remarks>
/// This class contains the essential information needed to create or update a company record.
/// All properties are required and must meet specific validation criteria.
/// </remarks>
public class CompanyRequest
{
    /// <summary>
    /// Gets or sets the legal name of the company.
    /// </summary>
    /// <remarks>
    /// The company name must be between 2 and 200 characters long and cannot be empty.
    /// </remarks>
    /// <example>Acme Corporation</example>
    [Required(ErrorMessage = "Company name is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Company name must be between 2 and 200 characters")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the CNPJ (Cadastro Nacional da Pessoa Jurídica) number.
    /// </summary>
    /// <remarks>
    /// CNPJ is the Brazilian company registration number, must contain exactly 14 numeric digits.
    /// Format should be numbers only, without special characters.
    /// </remarks>
    /// <example>12345678000199</example>
    [Required(ErrorMessage = "CNPJ is required")]
    [RegularExpression(@"^\d{14}$", ErrorMessage = "CNPJ must contain exactly 14 numeric digits")]
    public string Cnpj { get; set; } = null!;
}
