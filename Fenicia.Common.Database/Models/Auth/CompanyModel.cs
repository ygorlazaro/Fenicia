using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Common.Database.Models.Auth;

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

    [MaxLength(32)]
    [Column("logo")]
    public string? Logo { get; set; }

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

    [ForeignKey(nameof(CompanyModel.AddressId))]
    [JsonIgnore]
    public virtual AddressModel? Address
    {
        get; set;
    }

    [JsonIgnore]
    public virtual List<OrderModel> Orders { get; set; } = [];

    public static CompanyModel Convert(CompanyUpdateRequest company)
    {
        return new CompanyModel { Name = company.Name, TimeZone = company.Timezone };
    }

    public static List<CompanyResponse> Convert(List<UserRoleModel> userRoles)
    {
        return [.. userRoles.Select(ur => new CompanyResponse
        {
            Id = ur.Company.Id,
            Name = ur.Company.Name,
            Cnpj = ur.Company.Cnpj,
            Role = ur.Role
        })];
    }
}
