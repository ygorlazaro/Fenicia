using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Models;

[Table("modules", Schema = "auth")]
public class AuthModuleModel : BaseModel
{
    public AuthModuleModel()
    {
        this.Name = string.Empty;
    }

    [Required]
    [MaxLength(30)]
    [MinLength(3)]
    [Column("name")]
    public string Name { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    [Column("price")]
    [Precision(18, 2)]
    public decimal Price { get; set; }

    [Required]
    [Column("type")]
    [EnumDataType(typeof(ModuleType))]
    public ModuleType Type { get; set; }

    [JsonIgnore]
    public List<AuthSubscriptionCreditModel> SubscriptionCredits { get; set; } = [];

    [JsonIgnore]
    public List<AuthOrderDetailModel> OrderDetails { get; set; } = [];

    public List<AuthSubmoduleModel> Submodules { get; set; } = [];
}
