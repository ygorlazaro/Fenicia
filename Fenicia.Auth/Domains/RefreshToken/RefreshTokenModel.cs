namespace Fenicia.Auth.Domains.RefreshToken;

/// <summary>
/// Represents a refresh token model that contains information about the token, its expiration date, associated user ID, and its active status.
/// </summary>
public class RefreshTokenModel()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenModel"/> class with default values.
    /// </summary>
    /// <param name="token">The refresh token string.</param>
    /// <param name="expirationDate">The expiration date of the refresh token.</param>
    /// <param name="userId">The ID of the user associated with the refresh token.</param>
    /// <param name="isActive">A value indicating whether the refresh token is active.</param>
    public RefreshTokenModel(string token, DateTime expirationDate, Guid userId, bool isActive = true)
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
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expiration date of the refresh token.
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user associated with the refresh token.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the refresh token is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
