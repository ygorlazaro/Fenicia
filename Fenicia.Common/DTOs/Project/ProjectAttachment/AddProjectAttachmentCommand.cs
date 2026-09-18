using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectAttachment;

public class AddProjectAttachmentCommand()
{
    public AddProjectAttachmentCommand(
        [Required] Guid id,
        [Required] Guid taskId,
        [Required] [MaxLength(200)] string fileName,
        [Required] [MaxLength(200)] string fileUrl,
        long fileSize,
        [Required] Guid uploadedBy,
        [Required] [MaxLength(200)] string contentType)
        : this()
    {
        Id = id;
        TaskId = taskId;
        FileName = fileName;
        FileUrl = fileUrl;
        FileSize = fileSize;
        UploadedBy = uploadedBy;
        ContentType = contentType;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid TaskId { get; set; }

    [Required]
    [MaxLength(200)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FileUrl { get; set; } = string.Empty;

    public long FileSize { get; set; }

    [Required]
    public Guid UploadedBy { get; set; }

    [Required]
    [MaxLength(200)]
    public string ContentType { get; set; } = string.Empty;
}
