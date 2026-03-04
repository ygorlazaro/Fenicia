namespace Fenicia.Auth.Domains.User.ListUsers;

public record ListUsersResponse(
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPrevious,
    bool HasNext,
    List<UserListItemResponse> Users
);