namespace Fenicia.Auth.Domains.User.DTOs;

public record GetAllUsersQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);