using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data.Models;

[Table("subscription_credits", Schema = "auth")]
public class AuthSubscriptionCredit : BaseModel
{
    [Required]
    public Guid SubscriptionId { get; set; }

    [Required]
    public Guid ModuleId { get; set; }

    [Required]
    public bool IsActive { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public Guid? OrderDetailId { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(ModuleId))]
    public virtual AuthModule Module { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(SubscriptionId))]
    public virtual AuthSubscription Subscription { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(OrderDetailId))]
    public virtual AuthOrderDetail? OrderDetail { get; set; }
}
