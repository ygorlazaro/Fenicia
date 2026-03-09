using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Data.Models.Auth;

[Table("subscriptions", Schema = "auth")]
public class SubscriptionModel : BaseModel
{
    [Required]
    public SubscriptionStatus Status { get; set; }

    [Required]
    public Guid CompanyId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public Guid? OrderId { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public CompanyModel Company { get; set; } = null!;

    [ForeignKey(nameof(OrderId))]
    public virtual OrderModel? Order { get; set; }

    public virtual List<SubscriptionCreditModel> Credits { get; set; } = null!;
}
