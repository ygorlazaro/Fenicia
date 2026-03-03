namespace Fenicia.Module.Projects.Domains.ProjectSubtask.GetById;

public record GetProjectSubtaskByIdResponse(
    Guid Id,
    Guid TaskId,
    string Title,
    bool IsCompleted,
    int Order,
    DateTime? CompletedAt,
    Guid CompanyId);
