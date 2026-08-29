namespace Fenicia.Module.Projects.Domains.ProjectSubtask.DTOs;

public record UpdateProjectSubtaskCommand(Guid Id, Guid TaskId, string Title, bool IsCompleted, int Order, DateTime? CompletedAt);