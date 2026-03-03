namespace Fenicia.Module.Projects.Domains.ProjectComment.GetById;

public record GetProjectCommentByIdResponse(
    Guid Id,
    Guid TaskId,
    Guid UserId,
    string Content,
    Guid CompanyId);
