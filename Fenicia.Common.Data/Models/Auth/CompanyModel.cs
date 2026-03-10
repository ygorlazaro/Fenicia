using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Auth;

[Table("companies", Schema = "auth")]
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

    [Column("address_id")]
    public Guid? AddressId { get; set; }

    public List<UserRoleModel> UsersRoles { get; set; } = [];

    public List<SubscriptionModel> Subscriptions { get; set; } = null!;

    [ForeignKey(nameof(AddressId))]
    public AddressModel? Address { get; set; }

    public List<OrderModel> Orders { get; set; } = [];
    
    public List<ConfigurationModel> Configurations { get; set; } = [];
}
