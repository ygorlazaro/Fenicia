namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.GetAll;

public record GetAllProjectTaskAssigneeResponse(
    Guid Id,
    Guid TaskId,
    Guid UserId,
    string Role,
    DateTime AssignedAt,
    Guid CompanyId);
