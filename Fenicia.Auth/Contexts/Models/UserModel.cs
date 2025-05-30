using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Contexts.Models;

[Table("users")]
public class UserModel: BaseModel
{
    [EmailAddress]
    [Required]
    public string Email { get; set; } = null!;
    
    [Required]
    [JsonIgnore]
    public string Password { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];
    
    [JsonIgnore]
    public virtual CustomerModel? Customer { get; set; }
}