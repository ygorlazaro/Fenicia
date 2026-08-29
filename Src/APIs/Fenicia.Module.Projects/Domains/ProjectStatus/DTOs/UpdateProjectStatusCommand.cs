namespace Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;

public record UpdateProjectStatusCommand(Guid Id, Guid ProjectId, string Name, string Color, int Order, bool IsFinal);