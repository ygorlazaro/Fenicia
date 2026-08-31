namespace Fenicia.Module.SocialNetwork.Domains.Attachment.DTOs;

public record GetAttachmentResponse(Guid Id, string Url, string FileType, long FileSize, Guid CommentId, DateTime UploadDate);
