using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectAttachment;

public record DeleteProjectAttachmentCommand([Required] Guid Id);