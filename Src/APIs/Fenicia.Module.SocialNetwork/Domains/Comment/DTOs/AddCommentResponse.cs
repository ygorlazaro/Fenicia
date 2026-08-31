using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;

public record AddCommentResponse(
    [Required] Guid Id,
    [Required] Guid UserId,
    [Required] Guid FeedId,
    Guid? ParentCommentId,
    [Required][MaxLength(200)] string Text,
    [Required] DateTime CommentDate,
    [Required] Guid CompanyId);
