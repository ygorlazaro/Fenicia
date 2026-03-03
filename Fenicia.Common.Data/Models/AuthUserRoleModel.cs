using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data.Models;

[Table("users_roles", Schema = "auth")]
public class AuthUserRoleModel : BaseModel
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
    public AuthRoleModel RoleModel { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    [JsonIgnore]
    public AuthUserModel UserModel { get; set; } = null!;

    [ForeignKey(nameof(CompanyId))]
    [JsonIgnore]
    public AuthCompanyModel CompanyModel { get; set; } = null!;
}
