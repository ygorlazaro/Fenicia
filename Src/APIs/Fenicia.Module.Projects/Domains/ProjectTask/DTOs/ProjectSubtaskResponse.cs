using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectTask.DTOs;

public record ProjectSubtaskResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Title,
    bool IsCompleted,
    int Order,
    DateTime? DueDate);