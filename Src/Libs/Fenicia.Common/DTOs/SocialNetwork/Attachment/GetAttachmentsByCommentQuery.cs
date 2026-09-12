namespace Fenicia.Common.DTOs.SocialNetwork.Attachment;

public record GetAttachmentsByCommentQuery(
    int Page = 1,
    int PerPage = 10,
    string? Query = null,
    string? Sort = null);