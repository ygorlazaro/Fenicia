using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data.Models.Auth;

[Table("companies")]
public class CompanyModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    [Column("name")]
    public string Name { get; set; } = null!;

    [Required]
    [MinLength(14)]
    [MaxLength(14)]
    [Column("cnpj")]
    public string Cnpj { get; set; } = null!;

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Required]
    [MaxLength(256)]
    [Column("time_zone")]
    public string TimeZone { get; set; } = TimeZoneInfo.Local.StandardName;

    [Required]
    [MaxLength(10)]
    [Column("language")]
    public string Language { get; set; } = "pt-BR";

    [Column("address_id")]
    public Guid? AddressId
    {
        get; set;
    }

    [JsonIgnore]
    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];

    [JsonIgnore]
    public virtual List<SubscriptionModel> Subscriptions { get; set; } = null!;

    [ForeignKey(nameof(AddressId))]
    [JsonIgnore]
    public virtual AddressModel? Address
    {
        get; set;
    }

    [JsonIgnore]
    public virtual List<OrderModel> Orders { get; set; } = [];
}