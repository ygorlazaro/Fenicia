using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Position;

public class AddPositionCommand()
{
    public AddPositionCommand(string name)
        : this()
    {
        Name = name;
    }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
