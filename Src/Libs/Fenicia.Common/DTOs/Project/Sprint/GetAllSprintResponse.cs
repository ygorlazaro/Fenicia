namespace Fenicia.Common.DTOs.Project.Sprint;

public record GetAllSprintResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    DateTime? StartDate,
    DateTime? EndDate,
    string? Description,
    Guid CreatedBy,
    Guid CompanyId);
