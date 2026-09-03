using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Models.Auth;

[Table("modules", Schema = "auth")]
public class ModuleModel : BaseModel
{
    [Required]
    [MaxLength(30)]
    [MinLength(3)]
    [Column("name")]
    public string Name { get; init; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    [Column("price")]
    [Precision(18, 2)]
    public decimal Price { get; init; }

    [Required]
    [Column("type")]
    [EnumDataType(typeof(ModuleType))]
    public ModuleType Type { get; init; }

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; init; }

    [Column("icon")]
    [MaxLength(100)]
    public string? Icon { get; init; }

    [Required]
    [Column("is_active")]
    public bool IsActive { get; init; } = true;

    [Required]
    [Column("sort_order")]
    public int SortOrder { get; init; }

    public List<SubscriptionCreditModel> SubscriptionCredits { get; init; } = [];

    public List<OrderDetailModel> OrderDetails { get; init; } = [];
}