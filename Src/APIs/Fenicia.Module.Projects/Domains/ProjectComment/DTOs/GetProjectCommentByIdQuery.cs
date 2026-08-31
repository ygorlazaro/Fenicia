using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectComment.DTOs;

public record GetProjectCommentByIdQuery([Required] Guid Id);