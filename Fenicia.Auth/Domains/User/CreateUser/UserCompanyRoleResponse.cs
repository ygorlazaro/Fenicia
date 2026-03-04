namespace Fenicia.Auth.Domains.User.CreateUser;

public record UserCompanyRoleResponse(
    Guid CompanyId,
    string CompanyName,
    Guid RoleId,
    string RoleName
);