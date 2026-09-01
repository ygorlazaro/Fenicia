namespace Fenicia.Module.Projects.Domains.Project.DTOs;

public record GetAllProjectQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);
