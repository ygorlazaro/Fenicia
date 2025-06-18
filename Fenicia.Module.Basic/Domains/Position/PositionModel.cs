using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Database;
using Fenicia.Module.Basic.Domains.Employee;

namespace Fenicia.Module.Basic.Domains.Position;

[Table("positions")]
public class PositionModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    public virtual List<EmployeeModel> Employees { get; set; } = null!;
}
