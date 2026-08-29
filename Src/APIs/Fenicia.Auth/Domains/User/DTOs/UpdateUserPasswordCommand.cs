namespace Fenicia.Auth.Domains.User.DTOs;

public record UpdateUserPasswordCommand(Guid UserId, string Password);
