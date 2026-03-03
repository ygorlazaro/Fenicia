namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Update;

public record UpdateProjectTaskAssigneeResponse(
    Guid Id,
    Guid TaskId,
    Guid UserId,
    string Role,
    DateTime AssignedAt,
    Guid CompanyId);
