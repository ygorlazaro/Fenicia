using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Contexts.Models;

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

    public string? Logo { get; set; } = null!;

    public string TimeZone { get; set; } = TimeZoneInfo.Local.StandardName;

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
