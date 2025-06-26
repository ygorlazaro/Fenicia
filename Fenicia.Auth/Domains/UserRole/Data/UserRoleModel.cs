using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Auth.Domains.Company.Logic;
using Fenicia.Auth.Domains.Role.Data;
using Fenicia.Auth.Domains.User.Data;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Domains.UserRole.Data;

[Table("users_roles")]
public class UserRoleModel : BaseModel
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid RoleId { get; set; }

    [Required]
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
