using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.Sprint.DTOs;

public record DeleteSprintCommand([Required] Guid Id);
