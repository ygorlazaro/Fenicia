namespace Fenicia.Auth.Domains.RefreshToken.Responses;

public record ValidateTokenResponse(string Token, DateTime ExpirationDate, Guid UserId, bool IsActive);