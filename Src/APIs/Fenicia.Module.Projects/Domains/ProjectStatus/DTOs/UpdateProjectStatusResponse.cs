namespace Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;

public record UpdateProjectStatusResponse(Guid Id, Guid ProjectId, string Name, string Color, int Order, bool IsFinal, Guid CompanyId);