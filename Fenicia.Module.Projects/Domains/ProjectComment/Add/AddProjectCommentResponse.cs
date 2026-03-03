namespace Fenicia.Module.Projects.Domains.ProjectComment.Add;

public record AddProjectCommentResponse(
    Guid Id,
    Guid TaskId,
    Guid UserId,
    string Content,
    Guid CompanyId);
