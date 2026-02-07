using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data.Models.Auth;

[Table("users_roles")]
public class UserRoleModel : BaseModel
{
    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("role_id")]
    public Guid RoleId { get; set; }

    [Required]
    [Column("company_id")]
    public Guid CompanyId { get; set; }

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