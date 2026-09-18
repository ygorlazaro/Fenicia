using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Attachment;

public class AddAttachmentCommand()
{
    public AddAttachmentCommand(
        Guid id,
        string url,
        string fileType,
        long fileSize,
        Guid commentId)
        : this()
    {
        Id = id;
        Url = url;
        FileType = fileType;
        FileSize = fileSize;
        CommentId = commentId;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Url { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FileType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    [Required]
    public Guid CommentId { get; set; }
}
