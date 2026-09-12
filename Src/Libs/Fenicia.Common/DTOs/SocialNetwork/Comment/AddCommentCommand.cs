using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Comment;

public record AddCommentCommand(
    Guid Id,
    [Required] Guid ProfileId,
    [Required] Guid FeedId,
    Guid? ParentCommentId,
    [Required] [MaxLength(1024)] string Text);
