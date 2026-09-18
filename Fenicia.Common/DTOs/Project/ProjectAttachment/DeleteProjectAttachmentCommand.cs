using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectAttachment;

public class DeleteProjectAttachmentCommand()
{
    public DeleteProjectAttachmentCommand([Required] Guid id)
        : this()
    {
        Id = id;
    }

    [Required]
    public Guid Id { get; set; }
}
