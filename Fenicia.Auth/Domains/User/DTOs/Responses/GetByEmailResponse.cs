namespace Fenicia.Auth.Domains.User.DTOs.Responses;

public record GetByEmailResponse(Guid Id, string Email, string Name, string Password);