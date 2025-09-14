namespace Fenicia.Common.Database.Models.Basic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Database;

[Table("positions")]
public class PositionModel : BaseModel
{
    [Required]
    [MaxLength(length: 50)]
    public string Name { get; set; } = null!;

    public virtual List<EmployeeModel> Employees { get; set; } = null!;
}
