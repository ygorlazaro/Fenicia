namespace Fenicia.Module.Projects.Domains.ProjectTask.Update;

public record UpdateProjectTaskResponse(
    Guid Id,
    Guid ProjectId,
    Guid StatusId,
    string Title,
    string? Description,
    string Priority,
    string Type,
    int Order,
    int? EstimatePoints,
    DateTime? DueDate,
    Guid CreatedBy,
    Guid CompanyId);
