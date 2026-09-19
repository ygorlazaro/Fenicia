using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Role;

public class RoleResponse()
{
    public RoleResponse(Guid id, string name)
        : this()
    {
        Id = id;
        Name = name;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string Name { get; set; } = string.Empty;
}
