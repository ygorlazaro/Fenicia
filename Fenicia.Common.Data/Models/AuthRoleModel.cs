using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data.Models;

[Table("roles", Schema = "auth")]
public class AuthRoleModel : BaseModel
{
    [Required]
    [MaxLength(10)]
    [Column("name")]
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual List<AuthUserRoleModel> UsersRoles { get; set; } = [];
}
