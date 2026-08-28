namespace Fenicia.Auth.Domains.User.DTOs.Commands;

public record UpdateUserPasswordCommand(Guid UserId, string Password);
