namespace Fenicia.Auth.Domains.User.DTOs;

public record UpdateUserRoleCommand(Guid CompanyId, Guid RoleId);
