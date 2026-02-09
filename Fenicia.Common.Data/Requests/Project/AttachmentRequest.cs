namespace Fenicia.Common.Data.Requests.Project;

public class AttachmentRequest
{
    public Guid TaskId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public Guid UploadedBy { get; set; }
}