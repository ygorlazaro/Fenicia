using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Project;

public record GetProjectByIdQuery([Required] Guid Id);