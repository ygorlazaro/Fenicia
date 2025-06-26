using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Company.Logic;

/// <summary>
/// Response model containing company information
/// </summary>
public class CompanyResponse
{
    /// <summary>
    /// The unique identifier of the company
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid Id { get; set; }

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

    /// <summary>
    /// The preferred language for the company
    /// </summary>
    /// <example>pt-BR</example>
    public string Language { get; set; } = null!;

    /// <summary>
    /// The timezone of the company
    /// </summary>
    /// <example>America/Sao_Paulo</example>
    public string TimeZone { get; set; } = null!;
}
