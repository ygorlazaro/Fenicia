namespace Fenicia.Module.Projects.Domains.ProjectAttachment.GetAll;

public record GetAllProjectAttachmentResponse(
    Guid Id,
    Guid TaskId,
    string FileName,
    string FileUrl,
    long FileSize,
    Guid UploadedBy,
    Guid CompanyId);
