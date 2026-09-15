using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Subscription;

public class UserModuleResponse()
{
    public UserModuleResponse(Guid id, string name, ModuleType type)
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
    [EnumDataType(typeof(ModuleType))]
    public ModuleType Type { get; set; }
}
