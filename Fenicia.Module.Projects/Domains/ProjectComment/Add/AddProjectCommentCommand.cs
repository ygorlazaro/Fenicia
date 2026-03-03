namespace Fenicia.Module.Projects.Domains.ProjectComment.Add;

public record AddProjectCommentCommand(
    Guid Id,
    Guid TaskId,
    Guid UserId,
    string Content);
