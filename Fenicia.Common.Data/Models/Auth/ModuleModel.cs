using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Models.Auth;

[Table("modules")]
public sealed class ModuleModel : BaseModel
{
    public ModuleModel()
    {
        this.Name = string.Empty;
    }
    
    public ModuleModel(ModuleResponse response)
    {
        this.Id = response.Id;
        this.Name = response.Name;
        this.Price = response.Price;
        this.Type = response.Type;
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
    public List<SubscriptionCreditModel> SubscriptionCredits { get; set; } = [];

    [JsonIgnore]
    public List<OrderDetailModel> OrderDetails { get; set; } = [];

    public List<SubmoduleModel> Submodules { get; set; } = [];
}