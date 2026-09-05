using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.Sprint.DTOs;

public record UpdateSprintCommand(
    [Required] Guid Id,
    [Required] [MaxLength(256)] string Name,
    DateTime? StartDate,
    DateTime? EndDate,
    [MaxLength(4096)] string? Description);
