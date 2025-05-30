using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Contexts.Models;

[Table("roles")]
public class RoleModel : BaseModel
{
    [Required]
    public string Name { get; set; } = null!;
    
    [JsonIgnore]
    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];
}