namespace Fenicia.Module.Projects.Domains.Project.DTOs;

public record ProjectStatusResponse(Guid Id, string Name, string Color, int Order, bool IsFinal);