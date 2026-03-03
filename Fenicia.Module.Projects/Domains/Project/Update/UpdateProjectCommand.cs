namespace Fenicia.Module.Projects.Domains.Project.Update;

public record UpdateProjectCommand(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    DateTime? StartDate,
    DateTime? EndDate,
    Guid Owner);
