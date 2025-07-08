namespace Fenicia.Auth.Domains.User.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Common.Database;

using Order.Data;

using RefreshToken.Data;

using UserRole.Data;

[Table(name: "users")]
public class UserModel : BaseModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(maximumLength: 48, ErrorMessage = "Email cannot exceed 48 characters")]
    [Column(name: "email")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(maximumLength: 200, ErrorMessage = "Password hash cannot exceed 200 characters")]
    [JsonIgnore]
    [Column(name: "password")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(maximumLength: 48, ErrorMessage = "Name cannot exceed 48 characters")]
    [Column(name: "name")]
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];

    [JsonIgnore]
    public virtual List<RefreshTokenModel> RefreshTokens { get; set; } = [];

    [JsonIgnore]
    public virtual List<OrderModel> Orders { get; set; } = [];
}
