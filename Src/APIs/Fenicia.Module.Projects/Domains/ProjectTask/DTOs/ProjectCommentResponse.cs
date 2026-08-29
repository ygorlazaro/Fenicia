namespace Fenicia.Module.Projects.Domains.ProjectTask.DTOs;

public record ProjectCommentResponse(Guid Id, string Content, Guid AuthorId);