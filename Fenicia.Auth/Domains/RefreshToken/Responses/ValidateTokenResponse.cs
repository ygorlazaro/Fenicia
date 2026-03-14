namespace Fenicia.Auth.Domains.RefreshToken.Responses;

/// <summary>
/// Response containing refresh token validation data from Redis.
/// </summary>
public record ValidateTokenResponse(string Token, DateTime ExpirationDate, Guid UserId, bool IsActive);