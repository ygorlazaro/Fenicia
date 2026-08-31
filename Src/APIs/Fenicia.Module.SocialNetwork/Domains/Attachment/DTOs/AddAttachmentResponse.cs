using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Attachment.DTOs;

public record AddAttachmentResponse(
    [Required] Guid Id,
    [Required][MaxLength(200)] string Url,
    [Required][MaxLength(200)] string FileType,
    long FileSize,
    [Required] Guid CommentId,
    [Required] Guid CompanyId,
    [Required] DateTime UploadDate);
