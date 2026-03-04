namespace Fenicia.Auth.Domains.User.CreateUser;

public record UserCompanyRoleCommand(
    Guid CompanyId,
    Guid RoleId
);