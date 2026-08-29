namespace Fenicia.Auth.Domains.RefreshToken.DTOs;

public record ValidateTokenResponse(string Token, DateTime ExpirationDate, Guid UserId, bool IsActive);
