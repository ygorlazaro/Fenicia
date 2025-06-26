using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Auth.Domains.Module.Data;
using Fenicia.Auth.Domains.Order.Data;
using Fenicia.Auth.Domains.SubscriptionCredit.Data;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Domains.OrderDetail.Data;

/// <summary>
/// Represents the details of an order including module and pricing information
/// </summary>
[Table("order_details", Schema = "dbo")]
public class OrderDetailModel : BaseModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the associated order
    /// </summary>
    [Required(ErrorMessage = "Order ID is required")]
    [Column("order_id")]
    public Guid OrderId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the associated module
    /// </summary>
    [Required(ErrorMessage = "Module ID is required")]
    [Column("module_id")]
    public Guid ModuleId { get; set; }

    /// <summary>
    /// Gets or sets the monetary amount for this order detail
    /// </summary>
    [Required(ErrorMessage = "Amount is required")]
    [Column("amount", TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the associated order
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(OrderId))]
    public virtual OrderModel Order { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated module
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(ModuleId))]
    public virtual ModuleModel Module { get; set; } = null!;

    /// <summary>
    /// Gets or sets the optional subscription credit associated with this order detail
    /// </summary>
    [JsonIgnore]
    public virtual SubscriptionCreditModel? SubscriptionCredit { get; set; }
}
