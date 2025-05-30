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
    [Length(14, 14)]
    public string CNPJ { get; set; } = null!;

    [JsonIgnore]
    public virtual List<UserRoleModel> UsersRoles { get; set; } = [];

    [JsonIgnore]
    public virtual List<CustomerModel> Customers { get; set; } = null!;

    [JsonIgnore]
    public virtual List<SubscriptionModel> Subscriptions { get; set; } = null!;
}