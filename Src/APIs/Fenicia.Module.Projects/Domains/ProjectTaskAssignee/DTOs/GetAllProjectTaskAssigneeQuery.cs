namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.DTOs;

public record GetAllProjectTaskAssigneeQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);