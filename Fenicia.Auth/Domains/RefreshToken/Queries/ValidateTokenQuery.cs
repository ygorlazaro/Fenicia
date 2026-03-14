namespace Fenicia.Auth.Domains.RefreshToken.Queries;

/// <summary>
/// Query to validate a refresh token for a specific user.
/// </summary>
public record ValidateTokenQuery(Guid UserId, string RefreshToken);