namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.DTOs;

public record AddProjectTaskAssigneeCommand(Guid Id, Guid TaskId, Guid UserId, string Role, DateTime AssignedAt);