using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Contexts.Models;

[Table("users")]
public class UserModel : BaseModel
{
    [EmailAddress]
    [MaxLength(48)]
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    [JsonIgnore]
    [MaxLength(200)]
    public string Password { get; set; } = null!;

    [Required]
    [MaxLength(48)]
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];

    [JsonIgnore]
    public virtual CustomerModel? Customer { get; set; }
}