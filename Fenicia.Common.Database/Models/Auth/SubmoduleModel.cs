using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Common.Database.Responses;

namespace Fenicia.Common.Database.Models.Auth;

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

    public static SubmoduleResponse Convert(SubmoduleModel submodule)
    {
        return new SubmoduleResponse
        {
            Id = submodule.Id,
            Name = submodule.Name,
            Description = submodule.Description
        };
    }

    public static List<SubmoduleResponse> Convert(List<SubmoduleModel> submodules)
    {
        return submodules.Select(Convert).ToList();
    }
}
