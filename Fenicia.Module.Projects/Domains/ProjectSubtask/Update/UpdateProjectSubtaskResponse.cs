namespace Fenicia.Module.Projects.Domains.ProjectSubtask.Update;

public record UpdateProjectSubtaskResponse(
    Guid Id,
    Guid TaskId,
    string Title,
    bool IsCompleted,
    int Order,
    DateTime? CompletedAt,
    Guid CompanyId);
