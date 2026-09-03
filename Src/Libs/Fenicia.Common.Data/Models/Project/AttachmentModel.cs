using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.Project;

[Table("attachments", Schema = "project")]
public sealed class AttachmentModel : BaseCompanyModel
{
    public Guid TaskId { get; init; }

    [MaxLength(256)]
    public string FileName { get; init; } = string.Empty;

    [MaxLength(256)]
    public string FileUrl { get; init; } = string.Empty;

    public long FileSize { get; init; } = 0;

    public Guid UploadedBy { get; init; } = Guid.Empty;

    public ProjectTaskModel TaskModel { get; init; } = default!;

    public UserModel User { get; init; } = default!;

    [MaxLength(50)]
    public string? ContentType { get; init; }

    public long Size { get; init; }
}