using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectStatus;

public record GetProjectStatusByIdQuery([Required] Guid Id);