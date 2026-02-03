using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Common.Database.Responses;
using Fenicia.Common.Enums;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Database.Models.Auth;

[Table("modules")]
public class ModuleModel : BaseModel
{
    [Required]
    [MaxLength(30)]
    [MinLength(3)]
    [Column("name")]
    public string Name { get; set; } = null!;

    [Required]
    [Range(0.01, double.MaxValue)]
    [Column("amount")]
    [Precision(precision: 18, scale: 2)]
    public decimal Amount
    {
        get; set;
    }

    [Required]
    [Column("type")]
    [EnumDataType(typeof(ModuleType))]
    public ModuleType Type
    {
        get; set;
    }

    [JsonIgnore]
    public virtual List<SubscriptionCreditModel> SubscriptionCredits { get; set; } = [];

    [JsonIgnore]
    public virtual List<OrderDetailModel> OrderDetails { get; set; } = [];

    public virtual List<SubmoduleModel> Submodules { get; set; } = [];

    public static ModuleModel Convert(ModuleResponse module)
    {
        return new ModuleModel
        {
            Id = module.Id,
            Name = module.Name,
            Amount = module.Amount,
            Type = module.Type
        };
    }

    public static List<ModuleModel> Convert(List<ModuleResponse> modules)
    {
        return [.. modules.Select(Convert)];
    }
}
