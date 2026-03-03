namespace Fenicia.Module.Projects.Domains.ProjectComment.Update;

public record UpdateProjectCommentResponse(
    Guid Id,
    Guid TaskId,
    Guid UserId,
    string Content,
    Guid CompanyId);
