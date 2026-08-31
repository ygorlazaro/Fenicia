using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment.DTOs;

public record DeleteProjectAttachmentCommand([Required] Guid Id);