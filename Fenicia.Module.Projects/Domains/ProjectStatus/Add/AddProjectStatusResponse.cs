namespace Fenicia.Module.Projects.Domains.ProjectStatus.Add;

public record AddProjectStatusResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    string Color,
    int Order,
    bool IsFinal,
    Guid CompanyId);
