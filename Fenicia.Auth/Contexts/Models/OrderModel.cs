using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Fenicia.Auth.Enums;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Contexts.Models;

[Table("orders")]
public class OrderModel: BaseModel
{
    [Required]
    public Guid CustomerId { get; set; }
    
    [Required]
    public decimal TotalAmount { get; set; }
    
    [Required]
    public DateTime SaleDate { get; set; }
    
    [Required]
    public OrderStatus Status { get; set; }
    
    [JsonIgnore]
    [ForeignKey("CustomerId")]
    public virtual CustomerModel Customer { get; set; } = null!;
    
    [JsonIgnore]
    public virtual SubscriptionModel? Subscription { get; set; } = null!;
    
    public virtual List<OrderDetailModel> Details { get; set; } = null!;
}