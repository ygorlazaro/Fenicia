using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Project;

public record ProjectTaskResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Title,
    [MaxLength(200)] string? Description,
    [Required] [MaxLength(200)] string Priority,
    [Required] [MaxLength(200)] string Type,
    int? EstimatePoints,
    DateTime? DueDate);