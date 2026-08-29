namespace Fenicia.Module.Projects.Domains.ProjectTask.DTOs;

public record ProjectTaskAssigneeResponse(Guid Id, Guid UserId, string UserName, string UserEmail);