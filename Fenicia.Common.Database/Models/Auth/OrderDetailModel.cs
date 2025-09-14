namespace Fenicia.Common.Database.Models.Auth;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Database;

[Table("order_details")]
public class OrderDetailModel : BaseModel
{
    [Required(ErrorMessage = "Order ID is required")]
    [Column("order_id")]
    public Guid OrderId
    {
        get; set;
    }

    [Required(ErrorMessage = "Module ID is required")]
    [Column("module_id")]
    public Guid ModuleId
    {
        get; set;
    }

    [Required(ErrorMessage = "Amount is required")]
    [Column("amount", TypeName = "decimal(18,2)")]
    [Range(minimum: 0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount
    {
        get; set;
    }

    [JsonIgnore]
    [ForeignKey(nameof(OrderId))]
    public virtual OrderModel Order { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(ModuleId))]
    public virtual ModuleModel Module { get; set; } = null!;

    [JsonIgnore]
    public virtual SubscriptionCreditModel? SubscriptionCredit
    {
        get; set;
    }
}
