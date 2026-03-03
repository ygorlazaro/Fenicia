namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.GetById;

public record GetProjectTaskAssigneeByIdResponse(
    Guid Id,
    Guid TaskId,
    Guid UserId,
    string Role,
    DateTime AssignedAt,
    Guid CompanyId);
