namespace Fenicia.Auth.Domains.User.ListUsers;

public record UserListItemResponse(
    Guid Id,
    string Name,
    string Email,
    DateTime Created,
    DateTime? Updated,
    List<UserCompanyRoleResponse> Companies
);