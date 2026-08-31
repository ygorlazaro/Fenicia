namespace Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;

public record GetCommentByIdResponse(Guid Id, Guid UserId, Guid FeedId, Guid? ParentCommentId, string Text, DateTime CommentDate, DateTime? UpdatedDate);
