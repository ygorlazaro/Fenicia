namespace Fenicia.Module.Projects.Domains.ProjectTask.DTOs;

public record ProjectAttachmentResponse(Guid Id, string FileName, string ContentType, long Size);