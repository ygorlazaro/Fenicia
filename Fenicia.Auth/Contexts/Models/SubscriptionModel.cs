using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Fenicia.Auth.Enums;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Contexts.Models;

[Table("subscriptions")]
public class SubscriptionModel:BaseModel
{
    [Required]
    public SubscriptionStatus Status { get; set; }
    
    [Required]
    public Guid CompanyId { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [JsonIgnore]
    [ForeignKey("CompanyId")]
    public CompanyModel Company { get; set; } = null!;
    
    public virtual List<SubscriptionCreditModel> Customers { get; set; } = null!;
}