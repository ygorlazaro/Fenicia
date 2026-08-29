namespace Fenicia.Module.Projects.Domains.ProjectAttachment.DTOs;

public record AddProjectAttachmentCommand(Guid Id, Guid TaskId, string FileName, string FileUrl, long FileSize, Guid UploadedBy, string ContentType);