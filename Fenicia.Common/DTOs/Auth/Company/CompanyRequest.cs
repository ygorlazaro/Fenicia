using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Company;

/// <summary>
/// Request DTO for creating or updating a company.
/// </summary>
public class CompanyRequest()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyRequest"/> class with the specified company details.
    /// </summary>
    /// <param name="name">The name of the company.</param>
    /// <param name="cnpj">The company's CNPJ (Brazilian tax ID).</param>
    public CompanyRequest(string name, string cnpj)
        : this()
    {
        Name = name;
        Cnpj = cnpj;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the company (used for updates).
    /// </summary>
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
    [MaxLength(14)]
    public string Cnpj { get; set; } = string.Empty;
}
