namespace Fenicia.Auth.Domains.RefreshToken;

/// <summary>
///     Model representing a refresh token stored in Redis.
/// </summary>
public record RefreshTokenModel(string Token, DateTime ExpirationDate, Guid UserId, bool IsActive = true);