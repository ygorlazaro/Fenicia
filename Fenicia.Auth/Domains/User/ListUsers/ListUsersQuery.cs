namespace Fenicia.Auth.Domains.User.ListUsers;

public record ListUsersQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null
);