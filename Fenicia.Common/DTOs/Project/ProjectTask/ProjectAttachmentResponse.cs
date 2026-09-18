using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectTask;

public class ProjectAttachmentResponse()
{
    public ProjectAttachmentResponse(
        [Required] Guid id,
        [Required] [MaxLength(200)] string fileName,
        [Required] [MaxLength(200)] string contentType,
        long size)
        : this()
    {
        Id = id;
        FileName = fileName;
        ContentType = contentType;
        Size = size;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string ContentType { get; set; } = string.Empty;

    public long Size { get; set; }
}
