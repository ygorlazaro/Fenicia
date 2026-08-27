namespace Fenicia.Auth.Domains.RefreshToken.DTOs.Queries;

public record ValidateTokenQuery(Guid UserId, string RefreshToken);
