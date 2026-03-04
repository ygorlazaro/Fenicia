namespace Fenicia.Auth.Domains.User.UpdateUser;

public record UpdateUserResponse(
    Guid Id,
    string Name,
    string Email,
    DateTime Created,
    DateTime? Updated,
    List<UserCompanyRoleResponse> CompaniesRoles
);