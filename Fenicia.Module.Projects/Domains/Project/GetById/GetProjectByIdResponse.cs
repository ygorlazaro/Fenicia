namespace Fenicia.Module.Projects.Domains.Project.GetById;

public record GetProjectByIdResponse(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    DateTime? StartDate,
    DateTime? EndDate,
    Guid Owner,
    Guid CompanyId,
    List<ProjectStatusResponse> Statuses,
    List<ProjectTaskResponse> Tasks);