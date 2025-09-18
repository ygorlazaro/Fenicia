namespace Fenicia.Common.Database.Models.Auth;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Database;

[Table("users")]
public class UserModel : BaseModel
{
    [Required]
    [EmailAddress]
    [StringLength(48)]
    [Column("email")]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(200)]
    [JsonIgnore]
    [Column("password")]
    public string Password { get; set; } = null!;

    [Required]
    [StringLength(48)]
    [Column("name")]
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];

    [JsonIgnore]
    public virtual List<RefreshTokenModel> RefreshTokens { get; set; } = [];

    [JsonIgnore]
    public virtual List<OrderModel> Orders { get; set; } = [];
}
