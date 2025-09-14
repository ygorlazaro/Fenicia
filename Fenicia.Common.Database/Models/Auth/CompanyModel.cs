namespace Fenicia.Common.Database.Models.Auth;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Database;

using Requests;

[Table("companies")]
public class CompanyModel : BaseModel
{
    [Required(ErrorMessage = "Company name is required")]
    [MaxLength(length: 50, ErrorMessage = "Company name cannot exceed 50 characters")]
    [Display(Name = "Company Name")]
    [Column("name")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "CNPJ is required")]
    [MinLength(length: 14, ErrorMessage = "CNPJ must be 14 characters")]
    [MaxLength(length: 14, ErrorMessage = "CNPJ must be 14 characters")]
    [Display(Name = "CNPJ")]
    [Column("cnpj")]
    public string Cnpj { get; set; } = null!;

    [Required]
    [Display(Name = "Active Status")]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [MaxLength(length: 32, ErrorMessage = "Logo file name cannot exceed 32 characters")]
    [Display(Name = "Company Logo")]
    [Column("logo")]
    public string? Logo { get; set; } = null!;

    [Required]
    [MaxLength(length: 256, ErrorMessage = "Timezone cannot exceed 256 characters")]
    [Display(Name = "Time Zone")]
    [Column("time_zone")]
    public string TimeZone { get; set; } = TimeZoneInfo.Local.StandardName;

    [Required]
    [MaxLength(length: 10, ErrorMessage = "Language code cannot exceed 10 characters")]
    [Display(Name = "Language")]
    [Column("language")]
    public string Language { get; set; } = "pt-BR";

    [Display(Name = "Address ID")]
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

    public static CompanyModel Convert(CompanyUpdateRequest company)
    {
        return new CompanyModel { Name = company.Name, TimeZone = company.Timezone };
    }
}
