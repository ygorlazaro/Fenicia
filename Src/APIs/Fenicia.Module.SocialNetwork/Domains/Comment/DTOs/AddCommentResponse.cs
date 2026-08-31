namespace Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;

public record AddCommentResponse(Guid Id, Guid UserId, Guid FeedId, Guid? ParentCommentId, string Text, DateTime CommentDate, Guid CompanyId);
