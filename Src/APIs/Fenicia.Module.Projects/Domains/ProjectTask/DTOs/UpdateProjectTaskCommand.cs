using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectTask.DTOs;

public record UpdateProjectTaskCommand(
    [Required] Guid Id,
    [Required] Guid ProjectId,
    [Required] Guid StatusId,
    [Required] [MaxLength(200)] string Title,
    [MaxLength(200)] string? Description,
    [Required] [MaxLength(200)] string Priority,
    [Required] [MaxLength(200)] string Type,
    int Order,
    int? EstimatePoints,
    DateTime? DueDate,
    [Required] Guid CreatedBy);