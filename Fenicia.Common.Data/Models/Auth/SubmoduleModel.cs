using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data.Models.Auth;

[Table("submodules")]
public class SubmoduleModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    [Column("name")]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    [Column("route")]
    public string Route { get; set; } = null!;

    [Column("description")]
    [MaxLength(100)]
    public string? Description
    {
        get; set;
    }

    [Column("module_id")]
    [Required]
    public Guid ModuleId
    {
        get; set;
    }

    [ForeignKey(nameof(ModuleId))]
    [JsonIgnore]
    public virtual ModuleModel Module { get; set; } = null!;
}