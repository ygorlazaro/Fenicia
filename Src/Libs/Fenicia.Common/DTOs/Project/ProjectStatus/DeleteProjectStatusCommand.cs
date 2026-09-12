using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectStatus;

public record DeleteProjectStatusCommand([Required] Guid Id);