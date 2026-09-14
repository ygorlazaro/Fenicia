namespace Fenicia.Common.DTOs.Project.ProjectComment;

public record GetAllProjectCommentQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);