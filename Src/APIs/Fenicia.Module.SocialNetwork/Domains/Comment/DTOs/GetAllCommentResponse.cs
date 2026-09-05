using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;

public record GetAllCommentResponse(
    [Required] Guid Id,
    [Required] Guid ProfileId,
    [Required] Guid FeedId,
    Guid? ParentCommentId,
    [Required] [MaxLength(1024)] string Text,
    [Required] DateTime CommentDate,
    DateTime? UpdatedDate,
    int TotalLikes,
    int TotalReplies,
    bool IsMine);
