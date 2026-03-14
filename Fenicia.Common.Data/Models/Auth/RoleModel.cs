using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Auth;

[Table("roles", Schema = "auth")]
public class RoleModel : BaseModel
{
    [Required]
    [MaxLength(10)]
    [Column("name")]
    public string Name { get; set; } = null!;

    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];
}