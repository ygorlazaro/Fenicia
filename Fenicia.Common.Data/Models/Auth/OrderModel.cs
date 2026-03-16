using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Data.Models.Auth;

[Table("orders", Schema = "auth")]
public class OrderModel : BaseModel
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid CompanyId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime SaleDate { get; set; }

    [Required]
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus Status { get; set; }

    public virtual SubscriptionModel? Subscription { get; set; }

    public virtual ICollection<OrderDetailModel> Details { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public virtual UserModel User { get; set; } = null!;

    [ForeignKey(nameof(CompanyId))]
    public virtual CompanyModel Company { get; set; } = null!;
}