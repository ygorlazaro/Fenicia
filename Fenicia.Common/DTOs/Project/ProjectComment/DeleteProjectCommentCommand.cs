using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectComment;

public record DeleteProjectCommentCommand([Required] Guid Id);