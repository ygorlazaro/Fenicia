namespace Fenicia.Module.Projects.Domains.Project.GetById;

public record ProjectStatusResponse(
    Guid Id,
    string Name,
    string Color,
    int Order,
    bool IsFinal);