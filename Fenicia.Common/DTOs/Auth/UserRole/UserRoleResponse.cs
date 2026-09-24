using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.UserRole;

/// <summary>
/// Response DTO containing user role information with associated company details.
/// </summary>
public class UserRoleResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserRoleResponse"/> class with the specified user role and company details.
    /// </summary>
    /// <param name="id">The unique identifier of the user role.</param>
    /// <param name="role">The role name.</param>
    /// <param name="company">The company details associated with the user role.</param>
    public UserRoleResponse(Guid id, string role, CompanyResponse company)
        : this()
    {
        Id = id;
        Role = role;
        Company = company;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the user role.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the role name.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company details associated with the user role.
    /// </summary>
    [Required]
    public CompanyResponse Company { get; set; } = new();
}
