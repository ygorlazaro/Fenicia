using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectComment;

public record GetAllProjectCommentResponse(
    [Required] Guid Id,
    [Required] Guid TaskId,
    [Required] Guid UserId,
    [Required] string UserName,
    [Required] [MaxLength(4096)] string Content,
    DateTime Created,
    [Required] Guid CompanyId);