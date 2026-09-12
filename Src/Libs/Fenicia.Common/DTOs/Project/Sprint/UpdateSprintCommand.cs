using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Sprint;

public record UpdateSprintCommand(
    [Required] Guid Id,
    [Required] [MaxLength(256)] string Name,
    DateTime? StartDate,
    DateTime? EndDate,
    [MaxLength(4096)] string? Description);
