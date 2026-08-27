namespace Fenicia.Auth.Domains.RefreshToken;

public record RefreshTokenModel(string Token, DateTime ExpirationDate, Guid UserId, bool IsActive = true);