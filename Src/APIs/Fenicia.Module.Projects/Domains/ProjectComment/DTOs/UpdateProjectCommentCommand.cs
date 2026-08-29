namespace Fenicia.Module.Projects.Domains.ProjectComment.DTOs;

public record UpdateProjectCommentCommand(Guid Id, string Content);