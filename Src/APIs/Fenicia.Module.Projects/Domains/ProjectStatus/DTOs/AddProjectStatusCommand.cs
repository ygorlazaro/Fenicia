using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;

public record AddProjectStatusCommand([Required] Guid Id, [Required] Guid ProjectId, [Required][MaxLength(200)] string Name, [Required][MaxLength(200)] string Color, int Order, bool IsFinal);