namespace Fenicia.Module.Projects.Domains.Project.GetAll;

public record GetAllProjectResponse(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    DateTime? StartDate,
    DateTime? EndDate,
    Guid Owner,
    Guid CompanyId);
