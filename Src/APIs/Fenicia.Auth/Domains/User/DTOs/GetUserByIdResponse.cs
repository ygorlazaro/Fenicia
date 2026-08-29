namespace Fenicia.Auth.Domains.User.DTOs;

public record GetUserByIdResponse(Guid Id, string Name, string Email);
