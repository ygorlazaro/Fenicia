using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectAttachment;

public record GetProjectAttachmentByIdQuery([Required] Guid Id);