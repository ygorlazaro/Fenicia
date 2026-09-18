using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectSubtask;

public class DeleteProjectSubtaskCommand()
{
    public DeleteProjectSubtaskCommand([Required] Guid id)
        : this()
    {
        Id = id;
    }

    [Required]
    public Guid Id { get; set; }
}
