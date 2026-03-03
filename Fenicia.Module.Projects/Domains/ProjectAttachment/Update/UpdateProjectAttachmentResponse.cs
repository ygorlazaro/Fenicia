namespace Fenicia.Module.Projects.Domains.ProjectAttachment.Update;

public record UpdateProjectAttachmentResponse(
    Guid Id,
    Guid TaskId,
    string FileName,
    string FileUrl,
    long FileSize,
    Guid UploadedBy,
    Guid CompanyId);
