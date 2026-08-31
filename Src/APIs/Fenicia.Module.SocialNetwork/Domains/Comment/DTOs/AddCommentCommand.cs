using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;

public record AddCommentCommand(
    Guid Id,
    [Required] Guid UserId,
    [Required] Guid FeedId,
    Guid? ParentCommentId,
    [Required][MaxLength(1024)] string Text);
