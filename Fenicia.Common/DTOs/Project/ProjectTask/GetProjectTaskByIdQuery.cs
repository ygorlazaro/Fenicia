using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectTask;

public record GetProjectTaskByIdQuery([Required] Guid Id);