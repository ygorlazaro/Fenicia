using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Position;

public class GetAllPositionResponse() : ICrudItem
{
    public GetAllPositionResponse(Guid id, string name)
        : this()
    {
        Id = id;
        Name = name;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
