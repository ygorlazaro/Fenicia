namespace Fenicia.Common.Database.Models.Auth;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Database;

[Table("submodules")]
public class SubmoduleModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    [Column("name")]
    public string Name { get; set; } = default!;

    [Column("description")]
    [MaxLength(100)]
    public string? Description    { get; set;}

    [Column("module_id")]
    [Required]
       public int ModuleId
    {
        get; set;
    }

    public virtual ModuleModel Module { get; set; } = default!;
}
