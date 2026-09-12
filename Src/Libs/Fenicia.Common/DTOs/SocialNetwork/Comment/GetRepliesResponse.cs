using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Comment;

public record GetRepliesResponse(
    [Required] Guid Id,
    [Required] Guid ProfileId,
    [Required] Guid FeedId,
    Guid? ParentCommentId,
    [Required] [MaxLength(1024)] string Text,
    [Required] DateTime CommentDate,
    DateTime? UpdatedDate,
    int TotalLikes,
    bool IsMine);
