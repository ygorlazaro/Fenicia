namespace Fenicia.Module.Projects.Domains.ProjectSubtask.GetAll;

public record GetAllProjectSubtaskResponse(
    Guid Id,
    Guid TaskId,
    string Title,
    bool IsCompleted,
    int Order,
    DateTime? CompletedAt,
    Guid CompanyId);
