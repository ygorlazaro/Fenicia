using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Auth.Domains.User.Data;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Domains.RefreshToken.Data;

/// <summary>
/// Represents a refresh token entity in the authentication system.
/// Used for maintaining user sessions and implementing token-based authentication.
/// </summary>
[Table("refresh_tokens")]
public class RefreshTokenModel : BaseModel
{
    /// <summary>
    /// Gets or sets the refresh token string value.
    /// </summary>
    [Required(ErrorMessage = "Token is required")]
    [MaxLength(256, ErrorMessage = "Token cannot exceed 256 characters")]
    [Column("token")]
    public string Token { get; set; } = null!;

    /// <summary>
    /// Gets or sets the expiration date of the refresh token.
    /// Default value is set to 7 days from creation.
    /// </summary>
    [Required(ErrorMessage = "Expiration date is required")]
    [Column("expiration_date")]
    [DataType(DataType.DateTime)]
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(7);

    /// <summary>
    /// Gets or sets the ID of the user associated with this refresh token.
    /// </summary>
    [Required(ErrorMessage = "User ID is required")]
    [Column("user_id")]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets whether the refresh token is active.
    /// Inactive tokens cannot be used for authentication.
    /// </summary>
    [Required(ErrorMessage = "IsActive status is required")]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the associated user entity.
    /// This is a navigation property for Entity Framework.
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual UserModel User { get; set; } = null!;
}
