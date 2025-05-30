using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Auth.Enums;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Contexts.Models;

[Table("modules")]
public class ModuleModel : BaseModel
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public decimal Amount { get; set; }

    public ModuleType Type { get; set; }

    public virtual List<SubscriptionCreditModel> Customers { get; set; } = null!;

    public virtual List<OrderDetailModel> OrderDetails { get; set; } = null!;
}