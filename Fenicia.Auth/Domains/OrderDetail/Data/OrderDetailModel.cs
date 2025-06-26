namespace Fenicia.Auth.Domains.OrderDetail.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Common.Database;

using Module.Data;

using Order.Data;

using SubscriptionCredit.Data;

/// <summary>
///     Represents the details of an order including module and pricing information
/// </summary>
[Table(name: "order_details")]
public class OrderDetailModel : BaseModel
{
    /// <summary>
    ///     Gets or sets the unique identifier of the associated order
    /// </summary>
    [Required(ErrorMessage = "Order ID is required")]
    [Column(name: "order_id")]
    public Guid OrderId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the associated module
    /// </summary>
    [Required(ErrorMessage = "Module ID is required")]
    [Column(name: "module_id")]
    public Guid ModuleId { get; set; }

    /// <summary>
    ///     Gets or sets the monetary amount for this order detail
    /// </summary>
    [Required(ErrorMessage = "Amount is required")]
    [Column(name: "amount", TypeName = "decimal(18,2)")]
    [Range(minimum: 0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    /// <summary>
    ///     Gets or sets the associated order
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(OrderDetailModel.OrderId))]
    public virtual OrderModel Order { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the associated module
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(OrderDetailModel.ModuleId))]
    public virtual ModuleModel Module { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the optional subscription credit associated with this order detail
    /// </summary>
    [JsonIgnore]
    public virtual SubscriptionCreditModel? SubscriptionCredit { get; set; }
}
