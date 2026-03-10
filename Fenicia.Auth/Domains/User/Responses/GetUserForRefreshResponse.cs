namespace Fenicia.Auth.Domains.User.Responses;

public record GetUserForRefreshResponse(Guid Id, string Email, string Name);