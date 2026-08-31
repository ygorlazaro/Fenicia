using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectComment.DTOs;

public record GetAllProjectCommentQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);