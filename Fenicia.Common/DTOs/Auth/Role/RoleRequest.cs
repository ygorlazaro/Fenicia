using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Role;

/// <summary>
/// Request DTO for assigning a role to a user within a company.
/// </summary>
public class RoleRequest()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleRequest"/> class with the specified company and role identifiers.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="roleId">The unique identifier of the role.</param>
    public RoleRequest(Guid companyId, Guid roleId)
        : this()
    {
        CompanyId = companyId;
        RoleId = roleId;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the company.
    /// </summary>
    [Required]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the role.
    /// </summary>
    [Required]
    public Guid RoleId { get; set; }
}
