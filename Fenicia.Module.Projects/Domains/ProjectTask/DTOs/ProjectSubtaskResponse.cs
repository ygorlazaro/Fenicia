namespace Fenicia.Module.Projects.Domains.ProjectTask.DTOs;

public record ProjectSubtaskResponse(Guid Id, string Title, bool IsCompleted, int Order, DateTime? DueDate);