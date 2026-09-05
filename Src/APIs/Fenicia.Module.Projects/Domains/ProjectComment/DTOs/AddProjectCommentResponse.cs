using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectComment.DTOs;

public record AddProjectCommentResponse(
    [Required] Guid Id,
    [Required] Guid TaskId,
    [Required] Guid UserId,
    [Required] string UserName,
    [Required] [MaxLength(4096)] string Content,
    DateTime Created,
    [Required] Guid CompanyId);