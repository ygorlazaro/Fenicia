using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Comment;

public record GetCommentByIdResponse(
    [Required] Guid Id,
    [Required] Guid ProfileId,
    [Required] Guid FeedId,
    Guid? ParentCommentId,
    [Required] [MaxLength(200)] string Text,
    [Required] DateTime CommentDate,
    DateTime? UpdatedDate);
