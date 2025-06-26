using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Auth.Domains.UserRole.Data;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Domains.Role.Data;

/// <summary>
/// Represents a role entity in the authentication system.
/// </summary>
[Table("roles")]
public class RoleModel : BaseModel
{
    #region Properties

    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    /// <remarks>
    /// The name must be unique and cannot exceed 10 characters.
    /// </remarks>
    [Required(ErrorMessage = "Role name is required")]
    [MaxLength(10, ErrorMessage = "Role name cannot exceed 10 characters")]
    [Column("name")]
    [Display(Name = "Role Name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of user roles associated with this role.
    /// </summary>
    /// <remarks>
    /// This is a navigation property for the many-to-many relationship between users and roles.
    /// </remarks>
    [JsonIgnore]
    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];

    #endregion
}
