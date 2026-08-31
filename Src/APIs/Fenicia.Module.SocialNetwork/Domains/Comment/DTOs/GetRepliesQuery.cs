namespace Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;

public record GetRepliesQuery(int Page = 1, int PerPage = 10, Guid ParentCommentId = default);
