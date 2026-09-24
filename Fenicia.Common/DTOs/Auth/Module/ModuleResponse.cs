using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Module;

/// <summary>
/// Response DTO containing module details.
/// </summary>
public class ModuleResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleResponse"/> class with the specified module details.
    /// </summary>
    /// <param name="id">The unique identifier of the module.</param>
    /// <param name="name">The name of the module.</param>
    /// <param name="type">The type of the module.</param>
    /// <param name="description">The description of the module (optional).</param>
    /// <param name="isActive">Indicates whether the module is active.</param>
    /// <param name="sortOrder">The sort order of the module.</param>
    /// <param name="price">The price of the module (optional).</param>
    public ModuleResponse(
        Guid id,
        string name,
        EnumModuleType type,
        string? description,
        bool isActive,
        int sortOrder,
        decimal? price)
        : this()
    {
        Id = id;
        Name = name;
        Type = type;
        Description = description;
        IsActive = isActive;
        SortOrder = sortOrder;
        Price = price;
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

    /// <summary>
    /// Gets or sets the description of the module.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the module is active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the sort order of the module.
    /// </summary>
    [Required]
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets the price of the module.
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal? Price { get; set; }
}
