namespace Fenicia.Module.Projects.Domains.Project.Add;

public record AddProjectCommand(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    DateTime? StartDate,
    DateTime? EndDate,
    Guid Owner);
