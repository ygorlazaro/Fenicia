namespace Fenicia.Module.Projects.Domains.ProjectSubtask.Add;

public record AddProjectSubtaskResponse(
    Guid Id,
    Guid TaskId,
    string Title,
    bool IsCompleted,
    int Order,
    DateTime? CompletedAt,
    Guid CompanyId);
