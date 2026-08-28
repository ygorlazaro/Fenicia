namespace Fenicia.Module.Projects.Domains.ProjectComment.DTOs;

public record GetAllProjectCommentResponse(Guid Id, Guid TaskId, Guid UserId, string Content, Guid CompanyId);