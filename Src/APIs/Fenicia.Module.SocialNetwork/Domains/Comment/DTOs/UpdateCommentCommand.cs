using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;

public record UpdateCommentCommand(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Text);