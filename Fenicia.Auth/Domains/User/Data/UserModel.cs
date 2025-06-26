using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Auth.Domains.Order.Data;
using Fenicia.Auth.Domains.RefreshToken.Data;
using Fenicia.Auth.Domains.UserRole.Data;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Domains.User.Data;

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
    public virtual List<RefreshTokenModel> RefreshTokens { get; set; } = [];

    [JsonIgnore]
    public virtual List<OrderModel> Orders { get; set; } = [];
}
