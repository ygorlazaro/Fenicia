using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Module;

public class GetModuleResponse()
{
    public GetModuleResponse(
        Guid id,
        string name,
        ModuleType type,
        string? description,
        string? icon,
        bool isActive,
        int sortOrder,
        decimal? price)
        : this()
    {
        Id = id;
        Name = name;
        Type = type;
        Description = description;
        Icon = icon;
        IsActive = isActive;
        SortOrder = sortOrder;
        Price = price;
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

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }

    [Required]
    public bool IsActive { get; set; }

    [Required]
    public int SortOrder { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal? Price { get; set; }
}
