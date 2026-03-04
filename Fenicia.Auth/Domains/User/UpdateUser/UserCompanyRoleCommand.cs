namespace Fenicia.Auth.Domains.User.UpdateUser;

public record UserCompanyRoleCommand(
    Guid CompanyId,
    Guid RoleId
);