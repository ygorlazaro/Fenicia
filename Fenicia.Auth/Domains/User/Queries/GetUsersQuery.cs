namespace Fenicia.Auth.Domains.User.Queries;

public record GetUsersQuery(
    int Page = 1,
    int PerPage = 10
);