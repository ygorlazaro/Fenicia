namespace Fenicia.Auth.Domains.User.Commands;

public record CreateUserRoleCommand(Guid CompanyId, Guid RoleId);