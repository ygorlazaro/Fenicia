namespace Fenicia.Auth.Domains.User.ListUsers;

public record UserCompanyRoleResponse(
    Guid CompanyId,
    string CompanyName,
    Guid RoleId,
    string RoleName
);