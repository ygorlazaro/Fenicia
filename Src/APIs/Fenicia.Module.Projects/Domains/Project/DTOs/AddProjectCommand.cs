using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.Project.DTOs;

public record AddProjectCommand(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Title,
    [MaxLength(200)] string? Description,
    [Required] [MaxLength(200)] string Status,
    DateTime? StartDate,
    DateTime? EndDate,
    [Required] Guid Owner);