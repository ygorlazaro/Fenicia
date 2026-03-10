namespace Fenicia.Auth.Domains.User.Responses;

public record GetByEmailResponse(Guid Id, string Email, string Name, string Password);