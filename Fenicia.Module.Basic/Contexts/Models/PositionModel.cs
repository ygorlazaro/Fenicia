using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Database;

namespace Fenicia.Module.Basic.Contexts.Models;

[Table("positions")]
public class PositionModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;
    
    public virtual List<EmployeeModel> Employees { get; set; } = null!;
}