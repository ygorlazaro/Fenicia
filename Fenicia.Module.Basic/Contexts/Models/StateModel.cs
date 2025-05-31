using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Database;

namespace Fenicia.Module.Basic.Contexts.Models;

[Table("states")]
public class StateModel: BaseModel
{
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = null!;
    
    [Required]
    [MaxLength(2)]
    public string Uf { get; set; } = null!;
}