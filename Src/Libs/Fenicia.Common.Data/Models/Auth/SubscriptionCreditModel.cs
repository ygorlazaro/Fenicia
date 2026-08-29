using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Auth;

[Table("subscription_credits", Schema = "auth")]
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

    [ForeignKey(nameof(ModuleId))]
    public virtual ModuleModel Module { get; set; } = null!;

    [ForeignKey(nameof(SubscriptionId))]
    public virtual SubscriptionModel Subscription { get; set; } = null!;

    [ForeignKey(nameof(OrderDetailId))]
    public virtual OrderDetailModel? Order { get; set; }
}