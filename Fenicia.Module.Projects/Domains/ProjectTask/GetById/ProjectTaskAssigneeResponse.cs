namespace Fenicia.Module.Projects.Domains.ProjectTask.GetById;

public record ProjectTaskAssigneeResponse(
    Guid Id,
    Guid UserId,
    string UserName,
    string UserEmail);