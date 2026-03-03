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

public record ProjectStatusResponse(
    Guid Id,
    string Name,
    string Color,
    int Order,
    bool IsFinal);

public record ProjectTaskResponse(
    Guid Id,
    string Title,
    string? Description,
    string Priority,
    string Type,
    int? EstimatePoints,
    DateTime? DueDate);
