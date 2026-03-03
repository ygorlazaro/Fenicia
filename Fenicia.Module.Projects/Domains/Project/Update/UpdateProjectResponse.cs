namespace Fenicia.Module.Projects.Domains.Project.Update;

public record UpdateProjectResponse(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    DateTime? StartDate,
    DateTime? EndDate,
    Guid Owner,
    Guid CompanyId);
