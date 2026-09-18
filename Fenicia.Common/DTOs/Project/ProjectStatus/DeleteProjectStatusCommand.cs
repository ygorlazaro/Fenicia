using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectStatus;

public class DeleteProjectStatusCommand()
{
    public DeleteProjectStatusCommand([Required] Guid id)
        : this()
    {
        Id = id;
    }

    [Required]
    public Guid Id { get; set; }
}
