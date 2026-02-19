namespace Fenicia.Auth.Domains.RefreshToken.ValidateToken;

public record ValidateTokenQuery(Guid UserId, string RefreshToken);