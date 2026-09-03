using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Auth;

[Table("companies", Schema = "auth")]
public class CompanyModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MinLength(14)]
    [MaxLength(14)]
    [Column("cnpj")]
    public string Cnpj { get; init; } = string.Empty;

    [Required]
    [Column("is_active")]
    public bool IsActive { get; init; } = true;

    [Column("address_id")]
    public Guid? AddressId { get; init; }

    public List<UserRoleModel> UsersRoles { get; set; } = [];

    public List<SubscriptionModel> Subscriptions { get; set; } = [];

    [ForeignKey(nameof(AddressId))]
    public AddressModel? Address { get; init; }

    public List<OrderModel> Orders { get; init; } = [];

    public List<ConfigurationModel> Configurations { get; init; } = [];
}