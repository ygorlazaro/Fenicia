using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.State;

public class StateResponse()
{
    public StateResponse(Guid id, string name, string uf)
        : this()
    {
        Id = id;
        Name = name;
        Uf = uf;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(2)]
    public string Uf { get; set; } = string.Empty;
}
