namespace Fenicia.Auth.Domains.User.DTOs.Commands;

public record UpdatePasswordCommand(Guid UserId, string Password);
