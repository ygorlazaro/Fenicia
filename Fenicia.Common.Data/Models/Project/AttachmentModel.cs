using Fenicia.Common.Data.Requests.Project;

namespace Fenicia.Common.Data.Models.Project;

public class AttachmentModel(AttachmentRequest request) : BaseModel
{
    public Guid TaskId { get; set; } = request.TaskId;

    public string FileName { get; set; } = request.FileName;

    public string FileUrl { get; set; } = request.FileUrl;

    public long FileSize { get; set; } = request.FileSize;

    public Guid UploadedBy { get; set; } = request.UploadedBy;

    public virtual TaskModel Task { get; set; } = null!;

    public virtual UserModel User { get; set; } = null!;
}