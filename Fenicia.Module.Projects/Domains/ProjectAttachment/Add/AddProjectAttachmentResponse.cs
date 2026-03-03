namespace Fenicia.Module.Projects.Domains.ProjectAttachment.Add;

public record AddProjectAttachmentResponse(
    Guid Id,
    Guid TaskId,
    string FileName,
    string FileUrl,
    long FileSize,
    Guid UploadedBy,
    Guid CompanyId);
