using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Project;

public record ProjectStatusResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
    [Required] [MaxLength(200)] string Color,
    int Order,
    bool IsFinal);