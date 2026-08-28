namespace Fenicia.Auth.Domains.User.DTOs;

public record GetUserForRefreshResponse(Guid Id, string Email, string Name);
