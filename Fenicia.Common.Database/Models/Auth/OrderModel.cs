namespace Fenicia.Common.Database.Models.Auth;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Database;

using Enums;

[Table("orders")]
public class OrderModel : BaseModel
{
    [Required(ErrorMessage = "User ID is required")]
    public Guid UserId
    {
        get; set;
    }

    [Required(ErrorMessage = "Company ID is required")]
    public Guid CompanyId
    {
        get; set;
    }

    [Required(ErrorMessage = "Total amount is required")]
    [Column(TypeName = "decimal(18,2)")]
    [Range(minimum: 0, double.MaxValue, ErrorMessage = "Total amount must be greater than or equal to 0")]
    public decimal TotalAmount
    {
        get; set;
    }

    [Required(ErrorMessage = "Sale date is required")]
    [DataType(DataType.DateTime)]
    public DateTime SaleDate
    {
        get; set;
    }

    [Required(ErrorMessage = "Order status is required")]
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus Status
    {
        get; set;
    }

    [JsonIgnore]
    public virtual SubscriptionModel? Subscription
    {
        get; set;
    }

    [JsonIgnore]
    public virtual List<OrderDetailModel> Details { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    [JsonIgnore]
    public virtual UserModel User { get; set; } = null!;

    [ForeignKey(nameof(CompanyId))]
    [JsonIgnore]
    public virtual CompanyModel Company { get; set; } = null!;
}
