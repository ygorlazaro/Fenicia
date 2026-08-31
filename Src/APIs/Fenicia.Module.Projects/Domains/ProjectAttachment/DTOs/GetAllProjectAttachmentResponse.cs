using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment.DTOs;

public record GetAllProjectAttachmentResponse([Required] Guid Id, [Required] Guid TaskId, [Required][MaxLength(200)] string FileName, [Required][MaxLength(200)] string FileUrl, long FileSize, [Required] Guid UploadedBy, [Required] Guid CompanyId);