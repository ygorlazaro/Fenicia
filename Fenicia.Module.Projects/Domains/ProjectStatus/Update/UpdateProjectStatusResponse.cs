namespace Fenicia.Module.Projects.Domains.ProjectStatus.Update;

public record UpdateProjectStatusResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    string Color,
    int Order,
    bool IsFinal,
    Guid CompanyId);
