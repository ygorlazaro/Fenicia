using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.UserRole;

/// <summary>
/// Response DTO containing user company association details.
/// </summary>
public class UserCompanyResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserCompanyResponse"/> class with the specified user company details.
    /// </summary>
    /// <param name="id">The unique identifier of the user company association.</param>
    /// <param name="role">The user's role in the company.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="companyName">The name of the company.</param>
    /// <param name="cnpj">The company's CNPJ (Brazilian tax ID).</param>
    public UserCompanyResponse(Guid id, string role, Guid companyId, string companyName, string cnpj)
        : this()
    {
        Id = id;
        Role = role;
        CompanyId = companyId;
        CompanyName = companyName;
        Cnpj = cnpj;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the user company association.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user's role in the company.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the company.
    /// </summary>
    [Required]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Gets or sets the name of the company.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company's CNPJ (Brazilian tax ID).
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Cnpj { get; set; } = string.Empty;
}
