namespace Fenicia.Common.DTOs.Project.Project;

public record GetAllProjectQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);