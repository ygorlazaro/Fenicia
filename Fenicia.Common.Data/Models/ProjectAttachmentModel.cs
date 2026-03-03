using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("attachments", Schema = "project")]
public class ProjectAttachmentModel : BaseCompanyModel
{
    public Guid TaskId { get; set; }

    public string FileName { get; set; } = null!;

    public string FileUrl { get; set; } = null!;

    public long FileSize { get; set; } = 0;

    public Guid UploadedBy { get; set; } = Guid.Empty;

    public virtual ProjectTaskModel TaskModel { get; set; } = null!;

    public virtual AuthUserModel UserModel { get; set; } = null!;

    public string ContentType { get; set; }

    public long Size { get; set; }
}
