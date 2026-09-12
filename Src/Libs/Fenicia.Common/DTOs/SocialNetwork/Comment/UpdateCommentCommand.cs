using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Comment;

public record UpdateCommentCommand(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Text);