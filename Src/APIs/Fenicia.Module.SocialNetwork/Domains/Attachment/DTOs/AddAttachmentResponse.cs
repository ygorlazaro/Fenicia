namespace Fenicia.Module.SocialNetwork.Domains.Attachment.DTOs;

public record AddAttachmentResponse(Guid Id, string Url, string FileType, long FileSize, Guid CommentId, Guid CompanyId, DateTime UploadDate);
