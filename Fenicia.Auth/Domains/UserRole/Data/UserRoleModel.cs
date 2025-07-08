namespace Fenicia.Auth.Domains.UserRole.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Common.Database;

using Company.Data;

using Role.Data;

using User.Data;

[Table(name: "users_roles")]
public class UserRoleModel : BaseModel
{
    [Required(ErrorMessage = "User ID is required")]
    [Column(name: "user_id")]
    [Display(Name = "User ID")]
    public Guid UserId
    {
        get; set;
    }

    [Required(ErrorMessage = "Role ID is required")]
    [Column(name: "role_id")]
    [Display(Name = "Role ID")]
    public Guid RoleId
    {
        get; set;
    }

    [Required(ErrorMessage = "Company ID is required")]
    [Column(name: "company_id")]
    [Display(Name = "Company ID")]
    public Guid CompanyId
    {
        get; set;
    }

    [ForeignKey(nameof(UserRoleModel.RoleId))]
    [JsonIgnore]
    public virtual RoleModel Role { get; set; } = null!;

    [ForeignKey(nameof(UserRoleModel.UserId))]
    [JsonIgnore]
    public virtual UserModel User { get; set; } = null!;

    [ForeignKey(nameof(UserRoleModel.CompanyId))]
    [JsonIgnore]
    public virtual CompanyModel Company { get; set; } = null!;
}
