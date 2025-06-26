using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Auth.Domains.Company.Data;
using Fenicia.Auth.Domains.OrderDetail.Data;
using Fenicia.Auth.Domains.Subscription.Data;
using Fenicia.Auth.Domains.User.Data;
using Fenicia.Auth.Enums;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Domains.Order.Data;

/// <summary>
/// Represents an order in the system.
/// </summary>
[Table("orders")]
public class OrderModel : BaseModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the user who placed the order.
    /// </summary>
    [Required(ErrorMessage = "User ID is required")]
    [Display(Name = "User ID")]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the company associated with the order.
    /// </summary>
    [Required(ErrorMessage = "Company ID is required")]
    [Display(Name = "Company ID")]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Gets or sets the total amount of the order.
    /// </summary>
    [Required(ErrorMessage = "Total amount is required")]
    [Display(Name = "Total Amount")]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "Total amount must be greater than or equal to 0")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the date when the sale was made.
    /// </summary>
    [Required(ErrorMessage = "Sale date is required")]
    [Display(Name = "Sale Date")]
    [DataType(DataType.DateTime)]
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the current status of the order.
    /// </summary>
    [Required(ErrorMessage = "Order status is required")]
    [Display(Name = "Order Status")]
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the subscription associated with this order, if any.
    /// </summary>
    [JsonIgnore]
    public virtual SubscriptionModel? Subscription { get; set; }

    /// <summary>
    /// Gets or sets the list of order details.
    /// </summary>
    [JsonIgnore]
    public virtual List<OrderDetailModel> Details { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user who placed the order.
    /// </summary>
    [ForeignKey(nameof(UserId))]
    [JsonIgnore]
    public virtual UserModel User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the company associated with the order.
    /// </summary>
    [ForeignKey(nameof(CompanyId))]
    [JsonIgnore]
    public virtual CompanyModel Company { get; set; } = null!;
}
