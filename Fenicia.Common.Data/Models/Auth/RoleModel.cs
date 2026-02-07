using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data.Models.Auth;

[Table("roles")]
public class RoleModel : BaseModel
{
    [Required]
    [MaxLength(10)]
    [Column("name")]
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];
}