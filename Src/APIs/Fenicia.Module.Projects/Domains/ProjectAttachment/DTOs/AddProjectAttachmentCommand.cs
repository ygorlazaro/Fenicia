using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment.DTOs;

public record AddProjectAttachmentCommand(
    [Required] Guid Id,
    [Required] Guid TaskId,
    [Required] [MaxLength(200)] string FileName,
    [Required] [MaxLength(200)] string FileUrl,
    long FileSize,
    [Required] Guid UploadedBy,
    [Required] [MaxLength(200)] string ContentType);