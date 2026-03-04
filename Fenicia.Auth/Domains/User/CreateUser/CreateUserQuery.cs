namespace Fenicia.Auth.Domains.User.CreateUser;

public record CreateUserQuery(
    string Email,
    string Password,
    string Name,
    List<UserCompanyRoleCommand>? CompaniesRoles = null
);