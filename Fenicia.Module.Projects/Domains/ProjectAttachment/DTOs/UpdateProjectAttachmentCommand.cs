namespace Fenicia.Module.Projects.Domains.ProjectAttachment.DTOs;

public record UpdateProjectAttachmentCommand(Guid Id, Guid TaskId, string FileName, string FileUrl, long FileSize, Guid UploadedBy);