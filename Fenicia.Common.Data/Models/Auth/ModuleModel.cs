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
    public string Name { get; set; } = null!;

    [Required]
    [Range(0.01, double.MaxValue)]
    [Column("price")]
    [Precision(18, 2)]
    public decimal Price { get; set; }

    [Required]
    [Column("type")]
    [EnumDataType(typeof(ModuleType))]
    public ModuleType Type { get; set; }

    public List<SubscriptionCreditModel> SubscriptionCredits { get; set; } = [];

    public List<OrderDetailModel> OrderDetails { get; set; } = [];
}