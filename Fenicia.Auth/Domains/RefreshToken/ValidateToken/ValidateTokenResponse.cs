namespace Fenicia.Auth.Domains.RefreshToken.ValidateToken;

public record ValidateTokenResponse(string Token, DateTime ExpirationDate, Guid UserId, bool IsActive);