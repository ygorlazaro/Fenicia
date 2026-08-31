namespace Fenicia.Module.SocialNetwork.Domains.Attachment.DTOs;

public record AddAttachmentCommand(Guid Id, string Url, string FileType, long FileSize, Guid CommentId);
