using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.Sprint.DTOs;

public record AddSprintCommand(
    [Required] Guid Id,
    [Required] Guid ProjectId,
    [Required] [MaxLength(256)] string Name,
    DateTime? StartDate,
    DateTime? EndDate,
    [MaxLength(4096)] string? Description,
    [Required] Guid CreatedBy);
