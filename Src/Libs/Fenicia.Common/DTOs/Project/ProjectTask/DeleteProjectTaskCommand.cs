using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectTask;

public record DeleteProjectTaskCommand([Required] Guid Id);