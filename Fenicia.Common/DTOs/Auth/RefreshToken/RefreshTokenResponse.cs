using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.RefreshToken;

/// <summary>
/// Response DTO containing refresh token details.
/// </summary>
public class RefreshTokenResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenResponse"/> class with the specified token details.
    /// </summary>
    /// <param name="token">The refresh token string.</param>
    /// <param name="expirationDate">The expiration date of the refresh token.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="isActive">Indicates whether the refresh token is active.</param>
    public RefreshTokenResponse(string token, DateTime expirationDate, Guid userId, bool isActive)
        : this()
    {
        Token = token;
        ExpirationDate = expirationDate;
        UserId = userId;
        IsActive = isActive;
    }

    /// <summary>
    /// Gets or sets the refresh token string.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expiration date of the refresh token.
    /// </summary>
    [Required]
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the refresh token is active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; }
}
