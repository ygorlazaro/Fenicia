using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Role;

/// <summary>
/// Response DTO containing role information.
/// </summary>
public class RoleResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleResponse"/> class with the specified role details.
    /// </summary>
    /// <param name="id">The unique identifier of the role.</param>
    /// <param name="name">The name of the role.</param>
    public RoleResponse(Guid id, string name)
        : this()
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the role.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string Name { get; set; } = string.Empty;
}
