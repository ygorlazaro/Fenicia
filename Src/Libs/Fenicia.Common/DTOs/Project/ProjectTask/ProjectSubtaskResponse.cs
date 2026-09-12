using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectTask;

public record ProjectSubtaskResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Title,
    bool IsCompleted,
    int Order,
    DateTime? DueDate);