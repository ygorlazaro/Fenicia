namespace Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;

public record AddCommentCommand(Guid Id, Guid UserId, Guid FeedId, Guid? ParentCommentId, string Text);
