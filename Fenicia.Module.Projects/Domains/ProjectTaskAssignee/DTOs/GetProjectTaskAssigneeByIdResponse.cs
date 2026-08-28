namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.DTOs;

public record GetProjectTaskAssigneeByIdResponse(Guid Id, Guid TaskId, Guid UserId, string Role, DateTime AssignedAt, Guid CompanyId);