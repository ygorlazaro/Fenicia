namespace Fenicia.Auth.Domains.User.GetByEmail;

public record GetByEmailResponse(Guid Id, string Email, string Name, string Password);