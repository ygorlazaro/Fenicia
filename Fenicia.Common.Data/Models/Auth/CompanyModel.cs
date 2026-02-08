using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Common.Data.Requests.Auth;

namespace Fenicia.Common.Data.Models.Auth;

[Table("companies")]
public sealed class CompanyModel: BaseModel
{
    public CompanyModel()
    {
        this.Name = string.Empty;
        this.Cnpj = string.Empty;
        this.Id = Guid.Empty;
        this.TimeZone = string.Empty;
    }
    
    public CompanyModel(CompanyUpdateRequest request)
    {
        this.Name = request.Name;
        this.TimeZone = request.Timezone;
    }

    public CompanyModel(CompanyRequest request)
    {
        this.Name = request.Name;
        this.Cnpj = request.Cnpj;
        this.TimeZone = string.Empty;
    }


    [Required]
    [MaxLength(50)]
    [Column("name")]
    public string Name { get; set; }

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
    public string TimeZone { get; set; }

    [Required]
    [MaxLength(10)]
    [Column("language")]
    public string Language { get; set; } = "pt-BR";

    [Column("address_id")]
    public Guid? AddressId { get; set; }

    [JsonIgnore]
    public List<UserRoleModel> UsersRoles { get; set; } = [];

    [JsonIgnore]
    public List<SubscriptionModel> Subscriptions { get; set; } = null!;

    [ForeignKey(nameof(AddressId))]
    [JsonIgnore]
    public AddressModel? Address { get; set; }

    [JsonIgnore]
    public List<OrderModel> Orders { get; set; } = [];
}