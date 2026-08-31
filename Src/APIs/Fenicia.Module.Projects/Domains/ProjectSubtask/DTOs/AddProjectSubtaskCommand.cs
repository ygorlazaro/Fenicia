using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask.DTOs;

public record AddProjectSubtaskCommand([Required] Guid Id, [Required] Guid TaskId, [Required][MaxLength(200)] string Title, bool IsCompleted, int Order, DateTime? CompletedAt);