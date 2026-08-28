namespace Fenicia.Auth.Domains.User.DTOs.Commands;

public record CreateUserCommand(string Email, string Password, string Name, List<CreateUserRoleCommand>? Roles = null);
