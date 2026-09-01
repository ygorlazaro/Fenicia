using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.Project;

[Table("attachments", Schema = "project")]
public class AttachmentModel : BaseCompanyModel
{
    public Guid TaskId { get; set; }

    [MaxLength(256)]
    public string FileName { get; set; } = null!;

    [MaxLength(256)]
    public string FileUrl { get; set; } = null!;

    public long FileSize { get; set; } = 0;

    public Guid UploadedBy { get; set; } = Guid.Empty;

    public virtual ProjectTaskModel TaskModel { get; set; } = null!;

    public virtual UserModel User { get; set; } = null!;

    [MaxLength(50)]
    public string? ContentType { get; set; }

    public long Size { get; set; }
}