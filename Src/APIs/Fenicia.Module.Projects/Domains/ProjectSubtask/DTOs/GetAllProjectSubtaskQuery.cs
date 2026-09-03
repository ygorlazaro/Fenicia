namespace Fenicia.Module.Projects.Domains.ProjectSubtask.DTOs;

public record GetAllProjectSubtaskQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);