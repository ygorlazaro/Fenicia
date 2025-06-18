using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.OrderDetail;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Enums;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Domains.Order;

[Table("orders")]
public class OrderModel : BaseModel
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid CompanyId { get; set; }

    [Required]
    public decimal TotalAmount { get; set; }

    [Required]
    public DateTime SaleDate { get; set; }

    [Required]
    public OrderStatus Status { get; set; }

    [JsonIgnore]
    public virtual SubscriptionModel? Subscription { get; set; }

    [JsonIgnore]
    public virtual List<OrderDetailModel> Details { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    [JsonIgnore]
    public virtual UserModel User { get; set; } = null!;

    [ForeignKey(nameof(CompanyId))]
    [JsonIgnore]
    public virtual CompanyModel Company { get; set; } = null!;
}
