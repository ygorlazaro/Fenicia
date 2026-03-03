using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data.Models;

[Table("order_details", Schema = "auth")]
public class AuthOrderDetail : BaseModel
{
    [Required]
    [Column("order_id")]
    public Guid OrderId { get; set; }

    [Required]
    [Column("module_id")]
    public Guid ModuleId { get; set; }

    [Required]
    [Column("price", TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(OrderId))]
    public AuthOrder Order { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(ModuleId))]
    public AuthModule Module { get; set; } = null!;

    [JsonIgnore]
    public AuthSubscriptionCreditModel? SubscriptionCredit { get; set; }
}
