namespace Fenicia.Module.Projects.Domains.ProjectComment.DTOs;

public record AddProjectCommentCommand(Guid Id, Guid TaskId, Guid UserId, string Content);