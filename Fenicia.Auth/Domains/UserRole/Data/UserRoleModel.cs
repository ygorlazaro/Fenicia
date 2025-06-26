using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Auth.Domains.Company.Data;
using Fenicia.Auth.Domains.Role.Data;
using Fenicia.Auth.Domains.User.Data;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Domains.UserRole.Data;

/// <summary>
/// Represents the relationship between users and their roles within a company.
/// This model maps users to their assigned roles in specific companies.
/// </summary>
[Table("users_roles")]
public class UserRoleModel : BaseModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    [Required(ErrorMessage = "User ID is required")]
    [Column("user_id")]
    [Display(Name = "User ID")]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the role.
    /// </summary>
    [Required(ErrorMessage = "Role ID is required")]
    [Column("role_id")]
    [Display(Name = "Role ID")]
    public Guid RoleId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the company.
    /// </summary>
    [Required(ErrorMessage = "Company ID is required")]
    [Column("company_id")]
    [Display(Name = "Company ID")]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Gets or sets the associated role entity.
    /// This property represents the navigation to the role details.
    /// </summary>
    [ForeignKey(nameof(RoleId))]
    [JsonIgnore]
    public virtual RoleModel Role { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated user entity.
    /// This property represents the navigation to the user details.
    /// </summary>
    [ForeignKey(nameof(UserId))]
    [JsonIgnore]
    public virtual UserModel User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated company entity.
    /// This property represents the navigation to the company details.
    /// </summary>
    [ForeignKey(nameof(CompanyId))]
    [JsonIgnore]
    public virtual CompanyModel Company { get; set; } = null!;
}
