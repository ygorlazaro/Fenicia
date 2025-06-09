using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Fenicia.Common.Database;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Contexts.Models;

[Table("modules")]
public class ModuleModel : BaseModel
{
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = null!;

    [Required]
    public decimal Amount { get; set; }

    public ModuleType Type { get; set; }

    [JsonIgnore]
    public virtual List<SubscriptionCreditModel> SubscriptioCredits { get; set; } = null!;

    [JsonIgnore]
    public virtual List<OrderDetailModel> OrderDetails { get; set; } = null!;
}
