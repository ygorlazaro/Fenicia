using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("positions")]
public class PositionModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    public virtual List<EmployeeModel> Employees { get; set; } = null!;
}