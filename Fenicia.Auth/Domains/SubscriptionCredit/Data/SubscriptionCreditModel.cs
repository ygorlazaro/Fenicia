namespace Fenicia.Auth.Domains.SubscriptionCredit.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Common.Database;

using Module.Data;

using OrderDetail.Data;

using Subscription.Data;

[Table(name: "subscription_credits")]
public class SubscriptionCreditModel : BaseModel
{
    [Required(ErrorMessage = "Subscription ID is required")]
    [Display(Name = "Subscription")]
    public Guid SubscriptionId
    {
        get; set;
    }

    [Required(ErrorMessage = "Module ID is required")]
    [Display(Name = "Module")]
    public Guid ModuleId
    {
        get; set;
    }

    [Required(ErrorMessage = "Active status is required")]
    [Display(Name = "Is Active")]
    public bool IsActive
    {
        get; set;
    }

    [Required(ErrorMessage = "Start date is required")]
    [Display(Name = "Start Date")]
    [DataType(DataType.DateTime)]
    public DateTime StartDate
    {
        get; set;
    }

    [Required(ErrorMessage = "End date is required")]
    [Display(Name = "End Date")]
    [DataType(DataType.DateTime)]
    public DateTime EndDate
    {
        get; set;
    }

    [Display(Name = "Order Detail")]
    public Guid? OrderDetailId
    {
        get; set;
    }

    [JsonIgnore]
    [ForeignKey(nameof(SubscriptionCreditModel.ModuleId))]
    public virtual ModuleModel Module { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(SubscriptionCreditModel.SubscriptionId))]
    public virtual SubscriptionModel Subscription { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(SubscriptionCreditModel.OrderDetailId))]
    public virtual OrderDetailModel? OrderDetail
    {
        get; set;
    }
}
