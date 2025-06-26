using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Auth.Domains.Address;
using Fenicia.Auth.Domains.Order.Data;
using Fenicia.Auth.Domains.Subscription.Data;
using Fenicia.Auth.Domains.UserRole.Data;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Domains.Company.Data;

/// <summary>
/// Represents a company entity in the system.
/// </summary>
[Table("companies")]
public class CompanyModel : BaseModel
{
    /// <summary>
    /// Gets or sets the company name.
    /// </summary>
    [Required(ErrorMessage = "Company name is required")]
    [MaxLength(50, ErrorMessage = "Company name cannot exceed 50 characters")]
    [Display(Name = "Company Name")]
    [Column("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the CNPJ (Brazilian company registration number).
    /// </summary>
    [Required(ErrorMessage = "CNPJ is required")]
    [MaxLength(14, ErrorMessage = "CNPJ must be 14 characters")]
    [MinLength(14, ErrorMessage = "CNPJ must be 14 characters")]
    [Display(Name = "CNPJ")]
    [Column("cnpj")]
    public string Cnpj { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether the company is active.
    /// </summary>
    [Required]
    [Display(Name = "Active Status")]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the company logo file name.
    /// </summary>
    [MaxLength(32, ErrorMessage = "Logo file name cannot exceed 32 characters")]
    [Display(Name = "Company Logo")]
    [Column("logo")]
    public string? Logo { get; set; } = null!;

    /// <summary>
    /// Gets or sets the company's timezone.
    /// </summary>
    [Required]
    [MaxLength(256, ErrorMessage = "Timezone cannot exceed 256 characters")]
    [Display(Name = "Time Zone")]
    [Column("time_zone")]
    public string TimeZone { get; set; } = TimeZoneInfo.Local.StandardName;

    /// <summary>
    /// Gets or sets the company's preferred language.
    /// </summary>
    [Required]
    [MaxLength(10, ErrorMessage = "Language code cannot exceed 10 characters")]
    [Display(Name = "Language")]
    [Column("language")]
    public string Language { get; set; } = "pt-BR";

    /// <summary>
    /// Gets or sets the company's address identifier.
    /// </summary>
    [Display(Name = "Address ID")]
    [Column("address_id")]
    public Guid? AddressId { get; set; }

    /// <summary>
    /// Gets or sets the list of user roles associated with the company.
    /// </summary>
    [JsonIgnore]
    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of subscriptions associated with the company.
    /// </summary>
    [JsonIgnore]
    public virtual List<SubscriptionModel> Subscriptions { get; set; } = null!;

    /// <summary>
    /// Gets or sets the company's address details.
    /// </summary>
    [ForeignKey(nameof(AddressId))]
    [JsonIgnore]
    public virtual AddressModel? Address { get; set; }

    /// <summary>
    /// Gets or sets the list of orders associated with the company.
    /// </summary>
    [JsonIgnore]
    public virtual List<OrderModel> Orders { get; set; } = [];
}
