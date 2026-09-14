using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectComment;

public record GetProjectCommentByIdQuery([Required] Guid Id);