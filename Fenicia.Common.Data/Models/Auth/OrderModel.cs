using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.Data.Models.Auth;

[Table("orders", Schema = "auth")]
public sealed class OrderModel : BaseModel
{
    [Required]
    [Column("order_number")]
    [MaxLength(20)]
    public string OrderNumber { get; init; } = string.Empty;

    [Required]
    public Guid UserId { get; init; }

    [Required]
    public Guid CompanyId { get; init; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; init; }

    [Column("discount_amount")]
    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; init; }

    [Column("total_quantity")]
    [Range(0, int.MaxValue)]
    public int TotalQuantity { get; init; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime SaleDate { get; init; }

    [Required]
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus Status { get; init; }

    [Column("payment_method")]
    [EnumDataType(typeof(PaymentMethod))]
    public PaymentMethod PaymentMethod { get; init; }

    [Column("notes")]
    [MaxLength(1000)]
    public string? Notes { get; init; }

    public SubscriptionModel? Subscription { get; set; }

    public ICollection<OrderDetailModel> Details { get; init; } = [];

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; init; } = default!;

    [ForeignKey(nameof(CompanyId))]
    public CompanyModel Company { get; init; } = default!;
}