namespace Fenicia.Module.Projects.Domains.ProjectComment.Update;

public record UpdateProjectCommentCommand(
    Guid Id,
    string Content);
