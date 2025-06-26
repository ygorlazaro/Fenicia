using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Auth.Domains.UserRole.Data;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Domains.Role.Data;

[Table("roles")]
public class RoleModel : BaseModel
{
    [Required]
    [MaxLength(10)]
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];
}
