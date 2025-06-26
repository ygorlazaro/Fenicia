namespace Fenicia.Module.Basic.Domains.Position;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Common.Database;

using Employee;

[Table(name: "positions")]
public class PositionModel : BaseModel
{
    [Required]
    [MaxLength(length: 50)]
    public string Name { get; set; } = null!;

    public virtual List<EmployeeModel> Employees { get; set; } = null!;
}
