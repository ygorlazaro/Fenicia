using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Common.Enums;

namespace Fenicia.Common.Database.Models.Auth;

[Table("orders")]
public class OrderModel : BaseModel
{
    [Required]
    public Guid UserId
    {
        get; set;
    }

    [Required]
    public Guid CompanyId
    {
        get; set;
    }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal TotalAmount
    {
        get; set;
    }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime SaleDate
    {
        get; set;
    }

    [Required]
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

    [ForeignKey(nameof(OrderModel.UserId))]
    [JsonIgnore]
    public virtual UserModel User { get; set; } = null!;

    [ForeignKey(nameof(OrderModel.CompanyId))]
    [JsonIgnore]
    public virtual CompanyModel Company { get; set; } = null!;
}
