namespace Fenicia.Auth.Domains.User.DTOs;

public record CreateUserRoleCommand(Guid CompanyId, Guid RoleId);
