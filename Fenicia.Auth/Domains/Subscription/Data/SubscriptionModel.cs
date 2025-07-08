namespace Fenicia.Auth.Domains.Subscription.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Common.Database;

using Company.Data;

using Enums;

using Order.Data;

using SubscriptionCredit.Data;

[Table(name: "subscriptions")]
public class SubscriptionModel : BaseModel
{
    [Required(ErrorMessage = "Subscription status is required")]
    public SubscriptionStatus Status
    {
        get; set;
    }

    [Required(ErrorMessage = "Company ID is required")]
    public Guid CompanyId
    {
        get; set;
    }

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate
    {
        get; set;
    }

    [Required(ErrorMessage = "End date is required")]
    public DateTime EndDate
    {
        get; set;
    }

    public Guid? OrderId
    {
        get; set;
    }

    [JsonIgnore]
    [ForeignKey(nameof(SubscriptionModel.CompanyId))]
    public CompanyModel Company { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(SubscriptionModel.OrderId))]
    public virtual OrderModel? Order
    {
        get; set;
    }

    [JsonIgnore]
    public virtual List<SubscriptionCreditModel> Credits { get; set; } = null!;
}
