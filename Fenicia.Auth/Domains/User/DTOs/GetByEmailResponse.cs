namespace Fenicia.Auth.Domains.User.DTOs;

public record GetByEmailResponse(Guid Id, string Email, string Name, string Password);
