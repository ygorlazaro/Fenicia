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
    public string OrderNumber { get; set; } = null!;

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    [Column("discount_amount")]
    [Range(0, double.MaxValue)]
    public decimal DiscountAmount { get; set; }

    [Column("total_quantity")]
    [Range(0, int.MaxValue)]
    public int TotalQuantity { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime SaleDate { get; set; }

    [Required]
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus Status { get; set; }

    [Column("payment_method")]
    [EnumDataType(typeof(PaymentMethod))]
    public PaymentMethod PaymentMethod { get; set; }

    [Column("notes")]
    [MaxLength(1000)]
    public string? Notes { get; set; }

    public List<OrderDetailModel> Details { get; set; } = null!;

    [ForeignKey(nameof(CustomerId))]
    public CustomerModel Customer { get; set; } = null!;

    public Guid? EmployeeId { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public EmployeeModel? Employee { get; set; }
}
