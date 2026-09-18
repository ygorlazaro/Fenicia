using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Attachment;

public class GetAttachmentResponse()
{
    public GetAttachmentResponse(
        Guid id,
        string url,
        string fileType,
        long fileSize,
        Guid commentId,
        DateTime uploadDate)
        : this()
    {
        Id = id;
        Url = url;
        FileType = fileType;
        FileSize = fileSize;
        CommentId = commentId;
        UploadDate = uploadDate;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    [MaxLength(200)]
    public string Url { get; init; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FileType { get; init; } = string.Empty;

    public long FileSize { get; init; }

    [Required]
    public Guid CommentId { get; init; }

    [Required]
    public DateTime UploadDate { get; init; }
}
