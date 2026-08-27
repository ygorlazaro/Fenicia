namespace Fenicia.Auth.Domains.User.DTOs.Commands;

public record CreateUserRoleCommand(Guid CompanyId, Guid RoleId);