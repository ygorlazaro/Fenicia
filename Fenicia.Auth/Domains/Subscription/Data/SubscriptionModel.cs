namespace Fenicia.Auth.Domains.Subscription.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Common.Database;

using Company.Data;

using Enums;

using Order.Data;

using SubscriptionCredit.Data;

/// <summary>
///     Represents a subscription model in the system.
/// </summary>
[Table(name: "subscriptions")]
public class SubscriptionModel : BaseModel
{
    /// <summary>
    ///     Gets or sets the current status of the subscription.
    /// </summary>
    [Required(ErrorMessage = "Subscription status is required")]
    public SubscriptionStatus Status { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the company associated with this subscription.
    /// </summary>
    [Required(ErrorMessage = "Company ID is required")]
    public Guid CompanyId { get; set; }

    /// <summary>
    ///     Gets or sets the start date of the subscription.
    /// </summary>
    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    /// <summary>
    ///     Gets or sets the end date of the subscription.
    /// </summary>
    [Required(ErrorMessage = "End date is required")]
    public DateTime EndDate { get; set; }

    /// <summary>
    ///     Gets or sets the optional order identifier associated with this subscription.
    /// </summary>
    public Guid? OrderId { get; set; }

    /// <summary>
    ///     Gets or sets the associated company model.
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(SubscriptionModel.CompanyId))]
    public CompanyModel Company { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the associated order model.
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(SubscriptionModel.OrderId))]
    public virtual OrderModel? Order { get; set; }

    /// <summary>
    ///     Gets or sets the list of subscription credits associated with this subscription.
    /// </summary>
    [JsonIgnore]
    public virtual List<SubscriptionCreditModel> Credits { get; set; } = null!;
}
