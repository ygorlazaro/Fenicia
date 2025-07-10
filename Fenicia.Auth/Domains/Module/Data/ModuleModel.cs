namespace Fenicia.Auth.Domains.Module.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using Common.Database;
using Common.Enums;

using Microsoft.EntityFrameworkCore;

using OrderDetail.Data;

using SubscriptionCredit.Data;

[Table(name: "modules")]
public class ModuleModel : BaseModel
{
    [Required(ErrorMessage = "Module name is required")]
    [MaxLength(length: 30, ErrorMessage = "Module name cannot exceed 30 characters")]
    [MinLength(length: 3, ErrorMessage = "Module name must be at least 3 characters")]
    [Column(name: "name")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Amount is required")]
    [Range(minimum: 0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    [Column(name: "amount")]
    [Precision(precision: 18, scale: 2)]
    public decimal Amount
    {
        get; set;
    }

    [Required(ErrorMessage = "Module type is required")]
    [Column(name: "type")]
    [EnumDataType(typeof(ModuleType))]
    public ModuleType Type
    {
        get; set;
    }

    [JsonIgnore]
    [NotNull]
    public virtual List<SubscriptionCreditModel> SubscriptionCredits { get; set; } = [];

    [JsonIgnore]
    [NotNull]
    public virtual List<OrderDetailModel> OrderDetails { get; set; } = [];

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
        return modules.Select(ModuleModel.Convert).ToList();
    }
}
