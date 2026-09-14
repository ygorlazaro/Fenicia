using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectSubtask;

public record DeleteProjectSubtaskCommand([Required] Guid Id);