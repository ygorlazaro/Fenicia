using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Project;

public record DeleteProjectCommand([Required] Guid Id);