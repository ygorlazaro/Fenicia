using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectTask;

public record ProjectTaskAssigneeResponse(
    [Required] Guid Id,
    [Required] Guid UserId,
    [Required] [MaxLength(200)] string UserName,
    [Required] [MaxLength(200)] string UserEmail);