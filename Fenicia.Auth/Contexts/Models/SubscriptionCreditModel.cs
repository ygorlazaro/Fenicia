using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Contexts.Models;

[Table("subscription_credits")]
public class SubscriptionCreditModel : BaseModel
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
    public virtual ModuleModel Module { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(SubscriptionId))]
    public virtual SubscriptionModel Subscription { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(OrderDetailId))]
    public virtual OrderDetailModel? OrderDetail { get; set; }
}