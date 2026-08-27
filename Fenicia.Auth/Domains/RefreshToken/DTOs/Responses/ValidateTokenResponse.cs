namespace Fenicia.Auth.Domains.RefreshToken.DTOs.Responses;

public record ValidateTokenResponse(string Token, DateTime ExpirationDate, Guid UserId, bool IsActive);