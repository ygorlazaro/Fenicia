using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Data.Models.Auth;

[Table("subscriptions", Schema = "auth")]
public sealed class SubscriptionModel : BaseModel
{
    [Required]
    public SubscriptionStatus Status { get; init; }

    [Required]
    public Guid CompanyId { get; init; }

    [Required]
    public DateTime StartDate { get; init; }

    [Required]
    public DateTime EndDate { get; init; }

    public Guid? OrderId { get; init; }

    [ForeignKey(nameof(CompanyId))]
    public CompanyModel Company { get; init; } = default!;

    [ForeignKey(nameof(OrderId))]
    public OrderModel? Order { get; init; }

    public ICollection<SubscriptionCreditModel> Credits { get; set; } = [];
}