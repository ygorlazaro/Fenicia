using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Auth;

[Table("users_roles", Schema = "auth")]
public class UserRoleModel : BaseModel
{
    [Required]
    [Column("user_id")]
    public Guid UserId { get; init; }

    [Required]
    [Column("role_id")]
    public Guid RoleId { get; init; }

    [Required]
    [Column("company_id")]
    public Guid CompanyId { get; init; }

    [ForeignKey(nameof(RoleId))]
    public RoleModel Role { get; init; } = default!;

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; init; } = default!;

    [ForeignKey(nameof(CompanyId))]
    public CompanyModel Company { get; init; } = default!;
}