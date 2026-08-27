namespace Fenicia.Auth.Domains.User.DTOs.Responses;

public record GetUserByIdResponse(Guid Id, string Name, string Email);