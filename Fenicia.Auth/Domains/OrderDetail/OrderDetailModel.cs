using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.SubscriptionCredit;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Domains.OrderDetail;

[Table("order_details")]
public class OrderDetailModel : BaseModel
{
    [Required]
    public Guid OrderId { get; set; }

    [Required]
    public Guid ModuleId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(OrderId))]
    public virtual OrderModel Order { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(ModuleId))]
    public virtual ModuleModel Module { get; set; } = null!;

    [JsonIgnore]
    public virtual SubscriptionCreditModel? SubscriptionCredit { get; set; }
}
