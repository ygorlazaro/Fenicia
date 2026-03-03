using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data.Models;

[Table("users_roles", Schema = "auth")]
public class AuthUserRole : BaseModel
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
    public AuthRole Role { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    [JsonIgnore]
    public AuthUser User { get; set; } = null!;

    [ForeignKey(nameof(CompanyId))]
    [JsonIgnore]
    public AuthCompany Company { get; set; } = null!;
}
