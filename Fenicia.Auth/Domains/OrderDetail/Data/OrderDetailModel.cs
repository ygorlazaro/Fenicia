namespace Fenicia.Auth.Domains.OrderDetail.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Common.Database;

using Module.Data;

using Order.Data;

using SubscriptionCredit.Data;

[Table(name: "order_details")]
public class OrderDetailModel : BaseModel
{
    [Required(ErrorMessage = "Order ID is required")]
    [Column(name: "order_id")]
    public Guid OrderId
    {
        get; set;
    }

    [Required(ErrorMessage = "Module ID is required")]
    [Column(name: "module_id")]
    public Guid ModuleId
    {
        get; set;
    }

    [Required(ErrorMessage = "Amount is required")]
    [Column(name: "amount", TypeName = "decimal(18,2)")]
    [Range(minimum: 0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount
    {
        get; set;
    }

    [JsonIgnore]
    [ForeignKey(nameof(OrderDetailModel.OrderId))]
    public virtual OrderModel Order { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(OrderDetailModel.ModuleId))]
    public virtual ModuleModel Module { get; set; } = null!;

    [JsonIgnore]
    public virtual SubscriptionCreditModel? SubscriptionCredit
    {
        get; set;
    }
}
