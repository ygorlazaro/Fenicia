using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Sprint;

public class DeleteSprintCommand()
{
    public DeleteSprintCommand([Required] Guid id)
        : this()
    {
        Id = id;
    }

    [Required]
    public Guid Id { get; set; }
}
