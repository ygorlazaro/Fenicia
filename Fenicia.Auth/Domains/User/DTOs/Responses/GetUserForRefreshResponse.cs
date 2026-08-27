namespace Fenicia.Auth.Domains.User.DTOs.Responses;

public record GetUserForRefreshResponse(Guid Id, string Email, string Name);