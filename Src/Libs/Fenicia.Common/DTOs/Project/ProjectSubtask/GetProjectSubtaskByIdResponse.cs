using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectSubtask;

public record GetProjectSubtaskByIdResponse(
    [Required] Guid Id,
    [Required] Guid TaskId,
    [Required] [MaxLength(200)] string Title,
    bool IsCompleted,
    int Order,
    DateTime? CompletedAt,
    [Required] Guid CompanyId);