using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.SocialNetworkModels;

[Table("attachments", Schema = "social_network")]
public class AttachmentModel : BaseCompanyModel
{
    [Required]
    [MaxLength(512)]
    public string Url { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string FileType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    [Required]
    public Guid CommentId { get; set; }

    [ForeignKey(nameof(CommentId))]
    public CommentModel Comment { get; set; } = null!;

    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
}
