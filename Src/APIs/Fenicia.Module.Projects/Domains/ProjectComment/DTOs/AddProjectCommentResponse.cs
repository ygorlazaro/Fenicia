using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectComment.DTOs;

public record AddProjectCommentResponse([Required] Guid Id, [Required] Guid TaskId, [Required] Guid UserId, [Required][MaxLength(200)] string Content, [Required] Guid CompanyId);