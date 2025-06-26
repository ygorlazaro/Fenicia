namespace Fenicia.Auth.Domains.SubscriptionCredit.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Common.Database;

using Module.Data;

using OrderDetail.Data;

using Subscription.Data;

/// <summary>
///     Represents a subscription credit entity in the system.
///     This model handles the credit allocation for subscriptions per module.
/// </summary>
[Table(name: "subscription_credits")]
public class SubscriptionCreditModel : BaseModel
{
    /// <summary>
    ///     Gets or sets the unique identifier of the associated subscription.
    /// </summary>
    [Required(ErrorMessage = "Subscription ID is required")]
    [Display(Name = "Subscription")]
    public Guid SubscriptionId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier of the associated module.
    /// </summary>
    [Required(ErrorMessage = "Module ID is required")]
    [Display(Name = "Module")]
    public Guid ModuleId { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the subscription credit is active.
    /// </summary>
    [Required(ErrorMessage = "Active status is required")]
    [Display(Name = "Is Active")]
    public bool IsActive { get; set; }

    /// <summary>
    ///     Gets or sets the start date of the subscription credit.
    /// </summary>
    [Required(ErrorMessage = "Start date is required")]
    [Display(Name = "Start Date")]
    [DataType(DataType.DateTime)]
    public DateTime StartDate { get; set; }

    /// <summary>
    ///     Gets or sets the end date of the subscription credit.
    /// </summary>
    [Required(ErrorMessage = "End date is required")]
    [Display(Name = "End Date")]
    [DataType(DataType.DateTime)]
    public DateTime EndDate { get; set; }

    /// <summary>
    ///     Gets or sets the optional unique identifier of the associated order detail.
    /// </summary>
    [Display(Name = "Order Detail")]
    public Guid? OrderDetailId { get; set; }

    /// <summary>
    ///     Gets or sets the associated module entity.
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(SubscriptionCreditModel.ModuleId))]
    public virtual ModuleModel Module { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the associated subscription entity.
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(SubscriptionCreditModel.SubscriptionId))]
    public virtual SubscriptionModel Subscription { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the associated order detail entity.
    /// </summary>
    [JsonIgnore]
    [ForeignKey(nameof(SubscriptionCreditModel.OrderDetailId))]
    public virtual OrderDetailModel? OrderDetail { get; set; }
}
