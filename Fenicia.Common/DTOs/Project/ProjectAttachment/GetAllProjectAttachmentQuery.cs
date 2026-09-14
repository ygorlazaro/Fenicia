namespace Fenicia.Common.DTOs.Project.ProjectAttachment;

public record GetAllProjectAttachmentQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);