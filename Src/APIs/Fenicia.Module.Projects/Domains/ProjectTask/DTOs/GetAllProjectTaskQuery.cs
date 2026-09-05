namespace Fenicia.Module.Projects.Domains.ProjectTask.DTOs;

public record GetAllProjectTaskQuery(
    int Page = 1,
    int PerPage = 10,
    string? Query = null,
    string? Sort = null,
    Guid? StatusId = null,
    Guid? CreatedBy = null,
    Guid? AssigneeId = null,
    DateTime? DueFrom = null,
    DateTime? DueTo = null,
    string? Type = null,
    string? Priority = null);