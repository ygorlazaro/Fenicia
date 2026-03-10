namespace Fenicia.Auth.Domains.User.Responses;

public record UpdateUserResponse(
    Guid Id,
    string Name,
    string Email
);