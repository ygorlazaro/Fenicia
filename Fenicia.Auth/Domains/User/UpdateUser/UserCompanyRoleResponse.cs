namespace Fenicia.Auth.Domains.User.UpdateUser;

public record UserCompanyRoleResponse(
    Guid CompanyId,
    string CompanyName,
    Guid RoleId,
    string RoleName
);