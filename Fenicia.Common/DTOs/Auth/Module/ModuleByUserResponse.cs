using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Module;

public class ModuleByUserResponse()
{
    public ModuleByUserResponse(Guid id, string name, EnumModuleType type)
        : this()
    {
        Id = id;
        Name = name;
        Type = type;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(30)]
    [MinLength(3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EnumDataType(typeof(EnumModuleType))]
    public EnumModuleType Type { get; set; }
}
