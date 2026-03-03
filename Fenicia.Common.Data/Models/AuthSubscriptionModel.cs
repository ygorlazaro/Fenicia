using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Data.Models;

[Table("subscriptions", Schema = "auth")]
public class AuthSubscription : BaseModel
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

    [JsonIgnore]
    [ForeignKey(nameof(CompanyId))]
    public AuthCompany Company { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(OrderId))]
    public virtual AuthOrder? Order { get; set; }

    [JsonIgnore]
    public virtual List<AuthSubscriptionCreditModel> Credits { get; set; } = null!;
}
