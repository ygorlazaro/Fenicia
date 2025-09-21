namespace Fenicia.Common.Database.Models.Auth;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Database;

[Table("submodules")]
public class SubmoduleModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    [Column("name")]
    public string Name { get; set; } = default!;

    [Required]
    [MaxLength(50)]
    [Column("route")]
    public string Route { get; set; } = default!;

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
    public virtual ModuleModel Module { get; set; } = default!;
}
