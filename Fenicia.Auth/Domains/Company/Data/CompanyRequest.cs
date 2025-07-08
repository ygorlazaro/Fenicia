namespace Fenicia.Auth.Domains.Company.Data;

using System.ComponentModel.DataAnnotations;

public class CompanyRequest
{
    [Required(ErrorMessage = "Company name is required")]
    [StringLength(maximumLength: 200, MinimumLength = 2, ErrorMessage = "Company name must be between 2 and 200 characters")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "CNPJ is required")]
    [RegularExpression(pattern: @"^\d{14}$", ErrorMessage = "CNPJ must contain exactly 14 numeric digits")]
    public string Cnpj { get; set; } = null!;
}
