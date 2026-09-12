using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectStatus;

public record GetProjectStatusByIdResponse(
    [Required] Guid Id,
    [Required] Guid ProjectId,
    [Required] [MaxLength(200)] string Name,
    [Required] [MaxLength(200)] string Color,
    int Order,
    bool IsFinal,
    [Required] Guid CompanyId);