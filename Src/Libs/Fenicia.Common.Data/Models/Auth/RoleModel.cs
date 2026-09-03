using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Auth;

[Table("roles", Schema = "auth")]
public sealed class RoleModel : BaseModel
{
    [Required]
    [MaxLength(10)]
    [Column("name")]
    public string Name { get; init; } = string.Empty;

    public List<UserRoleModel> UsersRoles { get; init; } = [];
}