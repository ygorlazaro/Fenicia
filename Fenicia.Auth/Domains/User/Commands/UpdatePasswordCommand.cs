namespace Fenicia.Auth.Domains.User.Commands;

public record UpdatePasswordCommand(Guid UserId, string Password);