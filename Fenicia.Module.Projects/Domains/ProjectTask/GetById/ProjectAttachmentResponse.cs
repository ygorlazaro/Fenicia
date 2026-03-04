namespace Fenicia.Module.Projects.Domains.ProjectTask.GetById;

public record ProjectAttachmentResponse(
    Guid Id,
    string FileName,
    string ContentType,
    long Size);