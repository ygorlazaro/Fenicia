using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectTask.DTOs;

public record ProjectTaskAssigneeResponse(
    [Required] Guid Id,
    [Required] Guid UserId,
    [Required] [MaxLength(200)] string UserName,
    [Required] [MaxLength(200)] string UserEmail);