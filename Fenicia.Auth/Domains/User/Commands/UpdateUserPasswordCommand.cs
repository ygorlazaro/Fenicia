namespace Fenicia.Auth.Domains.User.Commands;

public record UpdateUserPasswordCommand(
    Guid UserId,
    string Password
);