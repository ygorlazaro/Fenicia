using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Contexts.Models;

[Table("users_roles")]
public class UserRoleModel : BaseModel
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid RoleId { get; set; }

    [Required]
    public Guid CompanyId { get; set; }

    [ForeignKey("RoleId")]
    [JsonIgnore]
    public virtual RoleModel Role { get; set; } = null!;

    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual UserModel User { get; set; } = null!;

    [ForeignKey("CompanyId")]
    [JsonIgnore]
    public virtual CompanyModel Company { get; set; } = null!;
}