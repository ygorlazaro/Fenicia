using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.SocialNetwork;

[Table("attachments", Schema = "social_network")]
public class AttachmentModel : BaseCompanyModel
{
    [Required]
    [MaxLength(512)]
    public string Url { get; init; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string FileType { get; init; } = string.Empty;

    public long FileSize { get; init; }

    [Required]
    public Guid CommentId { get; init; }

    [ForeignKey(nameof(CommentId))]
    public CommentModel Comment { get; init; } = default!;

    public DateTime UploadDate { get; init; } = DateTime.UtcNow;
}