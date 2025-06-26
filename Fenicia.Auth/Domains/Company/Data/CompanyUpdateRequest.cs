namespace Fenicia.Auth.Domains.Company.Data;

using System.ComponentModel.DataAnnotations;

/// <summary>
///     Request model for company update
/// </summary>
public class CompanyUpdateRequest
{
    /// <summary>
    ///     The name of the company
    /// </summary>
    /// <example>Acme Corporation</example>
    [MaxLength(length: 50)]
    [Required]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     The timezone of the company
    /// </summary>
    /// <example>America/Sao_Paulo</example>
    [Required]
    public string Timezone { get; set; } = null!;
}
