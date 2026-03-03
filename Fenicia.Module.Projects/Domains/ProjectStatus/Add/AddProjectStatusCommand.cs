namespace Fenicia.Module.Projects.Domains.ProjectStatus.Add;

public record AddProjectStatusCommand(
    Guid Id,
    Guid ProjectId,
    string Name,
    string Color,
    int Order,
    bool IsFinal);
