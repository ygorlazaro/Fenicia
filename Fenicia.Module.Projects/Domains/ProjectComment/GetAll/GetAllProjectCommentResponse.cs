namespace Fenicia.Module.Projects.Domains.ProjectComment.GetAll;

public record GetAllProjectCommentResponse(
    Guid Id,
    Guid TaskId,
    Guid UserId,
    string Content,
    Guid CompanyId);
