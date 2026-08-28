namespace Fenicia.Auth.Domains.User.DTOs;

public record UpdatePasswordCommand(Guid UserId, string Password);
