using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Attachment;

public record AddAttachmentCommand(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Url,
    [Required] [MaxLength(200)] string FileType,
    long FileSize,
    [Required] Guid CommentId);