namespace Fenicia.Auth.Domains.User.Commands;

public record UpdateUserRoleCommand(Guid CompanyId, Guid RoleId);