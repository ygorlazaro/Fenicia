namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Update;

public record UpdateProjectTaskAssigneeCommand(
    Guid Id,
    Guid TaskId,
    Guid UserId,
    string Role,
    DateTime AssignedAt);
