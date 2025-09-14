namespace Fenicia.Common.Database.Models.Auth;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Database;

[Table("users_roles")]
public class UserRoleModel : BaseModel
{
    [Required(ErrorMessage = "User ID is required")]
    [Column("user_id")]
    [Display(Name = "User ID")]
    public Guid UserId
    {
        get; set;
    }

    [Required(ErrorMessage = "Role ID is required")]
    [Column("role_id")]
    [Display(Name = "Role ID")]
    public Guid RoleId
    {
        get; set;
    }

    [Required(ErrorMessage = "Company ID is required")]
    [Column("company_id")]
    [Display(Name = "Company ID")]
    public Guid CompanyId
    {
        get; set;
    }

    [ForeignKey(nameof(RoleId))]
    [JsonIgnore]
    public virtual RoleModel Role { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    [JsonIgnore]
    public virtual UserModel User { get; set; } = null!;

    [ForeignKey(nameof(CompanyId))]
    [JsonIgnore]
    public virtual CompanyModel Company { get; set; } = null!;
}
