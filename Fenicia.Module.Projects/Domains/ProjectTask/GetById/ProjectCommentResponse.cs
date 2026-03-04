namespace Fenicia.Module.Projects.Domains.ProjectTask.GetById;

public record ProjectCommentResponse(
    Guid Id,
    string Content,
    Guid AuthorId);