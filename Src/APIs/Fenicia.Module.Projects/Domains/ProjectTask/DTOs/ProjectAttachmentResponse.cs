using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectTask.DTOs;

public record ProjectAttachmentResponse([Required] Guid Id, [Required][MaxLength(200)] string FileName, [Required][MaxLength(200)] string ContentType, long Size);