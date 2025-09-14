namespace Fenicia.Common.Database.Models.Auth;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Database;

[Table("subscription_credits")]
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
    [ForeignKey(nameof(ModuleId))]
    public virtual ModuleModel Module { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(SubscriptionId))]
    public virtual SubscriptionModel Subscription { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(OrderDetailId))]
    public virtual OrderDetailModel? OrderDetail
    {
        get; set;
    }
}
