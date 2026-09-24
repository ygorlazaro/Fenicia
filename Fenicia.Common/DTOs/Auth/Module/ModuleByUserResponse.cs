using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Module;

/// <summary>
/// Response DTO containing module information for user module listings.
/// </summary>
public class ModuleByUserResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleByUserResponse"/> class with the specified module details.
    /// </summary>
    /// <param name="id">The unique identifier of the module.</param>
    /// <param name="name">The name of the module.</param>
    /// <param name="type">The type of the module.</param>
    public ModuleByUserResponse(Guid id, string name, EnumModuleType type)
        : this()
    {
        Id = id;
        Name = name;
        Type = type;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the module.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the module.
    /// </summary>
    [Required]
    [MaxLength(30)]
    [MinLength(3)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the module.
    /// </summary>
    [Required]
    [EnumDataType(typeof(EnumModuleType))]
    public EnumModuleType Type { get; set; }
}
