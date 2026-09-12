using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Sprint;

public record DeleteSprintCommand([Required] Guid Id);
