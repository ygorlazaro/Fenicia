using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.Data.Models.Basic;

[Table("orders", Schema = "basic")]
public class OrderModel : BaseCompanyModel
{
    [Required]
    [Column("order_number")]
    [MaxLength(40)]
    public string OrderNumber { get; init; } = string.Empty;

    [Required]
    public Guid UserId { get; init; }

    [Required]
    public Guid CustomerId { get; init; }

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

    public List<OrderDetailModel> Details { get; init; } = [];

    [ForeignKey(nameof(CustomerId))]
    public CustomerModel Customer { get; init; } = default!;

    public Guid? EmployeeId { get; init; }

    [ForeignKey(nameof(EmployeeId))]
    public EmployeeModel? Employee { get; init; }
}