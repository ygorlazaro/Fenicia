namespace Fenicia.Common.Database.Models.Auth;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Database;

using Enums;

using Microsoft.EntityFrameworkCore;

using Responses;

[Table("modules")]
public class ModuleModel : BaseModel
{
    [Required(ErrorMessage = "Module name is required")]
    [MaxLength(length: 30, ErrorMessage = "Module name cannot exceed 30 characters")]
    [MinLength(length: 3, ErrorMessage = "Module name must be at least 3 characters")]
    [Column("name")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Amount is required")]
    [Range(minimum: 0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    [Column("amount")]
    [Precision(precision: 18, scale: 2)]
    public decimal Amount
    {
        get; set;
    }

    [Required(ErrorMessage = "Module type is required")]
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
        return [.. modules.Select(ModuleModel.Convert)];
    }
}
