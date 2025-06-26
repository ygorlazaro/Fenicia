using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Auth.Domains.Address;
using Fenicia.Auth.Domains.Order.Data;
using Fenicia.Auth.Domains.Subscription.Data;
using Fenicia.Auth.Domains.UserRole.Data;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Domains.Company.Logic;

[Table("companies")]
public class CompanyModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(14)]
    public string Cnpj { get; set; } = null!;

    [Required]
    public bool IsActive { get; set; } = true;

    [MaxLength(32)]
    public string? Logo { get; set; } = null!;

    [MaxLength(256)]
    public string TimeZone { get; set; } = TimeZoneInfo.Local.StandardName;

    [MaxLength(10)]
    public string Language { get; set; } = "pt-BR";

    public Guid? AddressId { get; set; }

    [JsonIgnore]
    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];

    [JsonIgnore]
    public virtual List<SubscriptionModel> Subscriptions { get; set; } = null!;

    [ForeignKey(nameof(AddressId))]
    [JsonIgnore]
    public virtual AddressModel? Address { get; set; }

    [JsonIgnore]
    public virtual List<OrderModel> Orders { get; set; } = [];
}
