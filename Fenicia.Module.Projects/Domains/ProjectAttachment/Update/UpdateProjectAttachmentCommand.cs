namespace Fenicia.Module.Projects.Domains.ProjectAttachment.Update;

public record UpdateProjectAttachmentCommand(
    Guid Id,
    Guid TaskId,
    string FileName,
    string FileUrl,
    long FileSize,
    Guid UploadedBy);
