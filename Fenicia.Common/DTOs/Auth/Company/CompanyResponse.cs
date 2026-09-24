using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Company;

/// <summary>
/// Response DTO containing company information with associated role.
/// </summary>
public class CompanyResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyResponse"/> class with the specified company details.
    /// </summary>
    /// <param name="id">The unique identifier of the company.</param>
    /// <param name="name">The name of the company.</param>
    /// <param name="cnpj">The company's CNPJ (Brazilian tax ID).</param>
    /// <param name="role">The role associated with the company (optional).</param>
    public CompanyResponse(Guid id, string name, string cnpj, string? role = null)
        : this()
    {
        Id = id;
        Name = name;
        Cnpj = cnpj;
        Role = role;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the company.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the company.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company's CNPJ (Brazilian tax ID).
    /// </summary>
    [Required]
    [Length(14, 14)]
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the role associated with the company.
    /// </summary>
    [MaxLength(10)]
    public string? Role { get; set; } = string.Empty;
}
