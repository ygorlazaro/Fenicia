using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;

public record UpdateCommentResponse(
    [Required] Guid Id,
    [Required] Guid ProfileId,
    [Required] Guid FeedId,
    Guid? ParentCommentId,
    [Required] [MaxLength(200)] string Text,
    [Required] DateTime CommentDate,
    DateTime? UpdatedDate,
    [Required] Guid CompanyId);
