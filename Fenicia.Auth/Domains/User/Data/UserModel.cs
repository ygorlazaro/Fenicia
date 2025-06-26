namespace Fenicia.Auth.Domains.User.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Common.Database;

using Order.Data;

using RefreshToken.Data;

using UserRole.Data;

/// <summary>
///     Represents a user entity in the system.
/// </summary>
[Table(name: "users")]
public class UserModel : BaseModel
{
    /// <summary>
    ///     Gets or sets the user's email address.
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(maximumLength: 48, ErrorMessage = "Email cannot exceed 48 characters")]
    [Column(name: "email")]
    public string Email { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the user's hashed password.
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(maximumLength: 200, ErrorMessage = "Password hash cannot exceed 200 characters")]
    [JsonIgnore]
    [Column(name: "password")]
    public string Password { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the user's full name.
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(maximumLength: 48, ErrorMessage = "Name cannot exceed 48 characters")]
    [Column(name: "name")]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the collection of user roles associated with this user.
    /// </summary>
    [JsonIgnore]
    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of refresh tokens associated with this user.
    /// </summary>
    [JsonIgnore]
    public virtual List<RefreshTokenModel> RefreshTokens { get; set; } = [];

    /// <summary>
    ///     Gets or sets the collection of orders associated with this user.
    /// </summary>
    [JsonIgnore]
    public virtual List<OrderModel> Orders { get; set; } = [];
}
