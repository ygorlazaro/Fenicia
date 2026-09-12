using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectSubtask;

public record GetProjectSubtaskByIdQuery([Required] Guid Id);