namespace Fenicia.Auth.Domains.User.Responses;

public record GetUserByIdResponse(Guid Id, string Name, string Email);