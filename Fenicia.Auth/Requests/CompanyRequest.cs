using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Requests;

/// <summary>
/// Request model for company creation or update
/// </summary>
public class CompanyRequest
{
    /// <summary>
    /// The name of the company
    /// </summary>
    /// <example>Acme Corporation</example>
    [MaxLength(50)]
    [Required]
    public string Name { get; set; } = null!;

    /// <summary>
    /// The CNPJ (Brazilian company registration number)
    /// </summary>
    /// <example>12345678000199</example>
    [Required]
    [MaxLength(14)]
    public string Cnpj { get; set; } = null!;
}