using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.DTOs;

public record GetAllProjectTaskAssigneeResponse([Required] Guid Id, [Required] Guid TaskId, [Required] Guid UserId, [Required][MaxLength(200)] string Role, [Required] DateTime AssignedAt, [Required] Guid CompanyId);