namespace Fenicia.Auth.Domains.User.UpdateUser;

public record UpdateUserQuery(
    Guid UserId,
    string? Name = null,
    string? Email = null,
    List<UserCompanyRoleCommand>? CompaniesRoles = null
);