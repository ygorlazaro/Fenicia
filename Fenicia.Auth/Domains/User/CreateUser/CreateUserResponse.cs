namespace Fenicia.Auth.Domains.User.CreateUser;

public record CreateUserResponse(
    Guid Id,
    string Name,
    string Email,
    DateTime Created,
    List<UserCompanyRoleResponse> CompaniesRoles
);