namespace Fenicia.Module.Projects.Domains.Project.Add;

public record AddProjectResponse(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    DateTime? StartDate,
    DateTime? EndDate,
    Guid Owner,
    Guid CompanyId);
