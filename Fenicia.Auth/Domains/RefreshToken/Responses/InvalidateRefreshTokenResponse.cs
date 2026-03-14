namespace Fenicia.Auth.Domains.RefreshToken.Responses;

/// <summary>
/// Response model for refresh token invalidation.
/// </summary>
public record InvalidateRefreshTokenResponse(string Token, DateTime ExpirationDate, Guid UserId)
{
    /// <summary>
    /// Indicates whether the token is still active. Set to false when invalidated.
    /// </summary>
    public bool IsActive { get; set; }
}