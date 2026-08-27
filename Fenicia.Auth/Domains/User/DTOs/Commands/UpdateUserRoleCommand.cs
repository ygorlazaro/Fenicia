namespace Fenicia.Auth.Domains.User.DTOs.Commands;

public record UpdateUserRoleCommand(Guid CompanyId, Guid RoleId);