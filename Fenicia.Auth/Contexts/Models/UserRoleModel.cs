using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Contexts.Models;

[Table("users_roles")]
public class UserRoleModel : BaseModel
{
    public Guid UserId { get; set; }
    
    public Guid RoleId { get; set; }
    
    [ForeignKey("RoleId")]
    public virtual RoleModel Role { get; set; } = null!;
    
    [ForeignKey("UserId")]
    public virtual UserModel User { get; set; } = null!;
}