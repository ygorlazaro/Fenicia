using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Auth;

[Table("subscription_credits", Schema = "auth")]
public sealed class SubscriptionCreditModel : BaseModel
{
    [Required]
    public Guid SubscriptionId { get; init; }

    [Required]
    public Guid ModuleId { get; init; }

    [Required]
    public bool IsActive { get; init; }

    [Required]
    public DateTime StartDate { get; init; }

    [Required]
    public DateTime EndDate { get; init; }

    public Guid? OrderDetailId { get; init; }

    [ForeignKey(nameof(ModuleId))]
    public ModuleModel Module { get; init; } = default!;

    [ForeignKey(nameof(SubscriptionId))]
    public SubscriptionModel Subscription { get; init; } = default!;

    [ForeignKey(nameof(OrderDetailId))]
    public OrderDetailModel? Order { get; init; }
}