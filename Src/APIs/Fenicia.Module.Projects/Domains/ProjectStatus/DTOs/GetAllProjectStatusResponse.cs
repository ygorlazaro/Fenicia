namespace Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;

public record GetAllProjectStatusResponse(Guid Id, Guid ProjectId, string Name, string Color, int Order, bool IsFinal, Guid CompanyId);