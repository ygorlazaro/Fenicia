using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectAttachment;

public class GetProjectAttachmentByIdQuery()
{
    public GetProjectAttachmentByIdQuery([Required] Guid id)
        : this()
    {
        Id = id;
    }

    [Required]
    public Guid Id { get; set; }
}
