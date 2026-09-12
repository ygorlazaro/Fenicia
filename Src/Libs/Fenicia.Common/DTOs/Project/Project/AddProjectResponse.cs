using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Project;

public record AddProjectResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Title,
    [MaxLength(200)] string? Description,
    [Required] [MaxLength(200)] string Status,
    DateTime? StartDate,
    DateTime? EndDate,
    [Required] Guid Owner,
    [Required] Guid CompanyId);