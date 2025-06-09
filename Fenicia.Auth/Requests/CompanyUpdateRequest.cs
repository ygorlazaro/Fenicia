using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Requests;

/// <summary>
/// Request model for company update
/// </summary>
public class CompanyUpdateRequest
{
    /// <summary>
    /// The name of the company
    /// </summary>
    /// <example>Acme Corporation</example>
    [MaxLength(50)]
    [Required]
    public string Name { get; set; } = null!;

    public string Timezone { get; set; } = null!;
}
