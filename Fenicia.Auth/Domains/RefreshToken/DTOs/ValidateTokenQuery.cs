namespace Fenicia.Auth.Domains.RefreshToken.DTOs;

public record ValidateTokenQuery(Guid UserId, string RefreshToken);
