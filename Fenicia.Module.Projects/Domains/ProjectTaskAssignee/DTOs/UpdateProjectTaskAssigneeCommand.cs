namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.DTOs;

public record UpdateProjectTaskAssigneeCommand(Guid Id, Guid TaskId, Guid UserId, string Role, DateTime AssignedAt);