using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("attachments", Schema = "project")]
public class ProjectAttachment : BaseCompanyModel
{
    public Guid TaskId { get; set; }

    public string FileName { get; set; } = null!;

    public string FileUrl { get; set; } = null!;

    public long FileSize { get; set; } = 0;

    public Guid UploadedBy { get; set; } = Guid.Empty;

    public virtual ProjectTask Task { get; set; } = null!;

    public virtual AuthUser User { get; set; } = null!;
}
