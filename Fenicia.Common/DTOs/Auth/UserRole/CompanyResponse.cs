using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.UserRole;

/// <summary>
/// Response DTO containing company information for user role associations.
/// </summary>
public class CompanyResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyResponse"/> class.
    /// </summary>
    public CompanyResponse()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyResponse"/> class with the specified company details.
    /// </summary>
    /// <param name="id">The unique identifier of the company.</param>
    /// <param name="name">The name of the company.</param>
    /// <param name="cnpj">The company's CNPJ (Brazilian tax ID).</param>
    /// <param name="isActive">Indicates whether the company is active (default: true).</param>
    public CompanyResponse(Guid id, string name, string cnpj, bool isActive = true)
    {
        Id = id;
        Name = name;
        Cnpj = cnpj;
        IsActive = isActive;
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
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company's CNPJ (Brazilian tax ID).
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the company is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
