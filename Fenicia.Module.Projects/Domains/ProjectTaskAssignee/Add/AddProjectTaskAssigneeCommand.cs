namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Add;

public record AddProjectTaskAssigneeCommand(
    Guid Id,
    Guid TaskId,
    Guid UserId,
    string Role,
    DateTime AssignedAt);
