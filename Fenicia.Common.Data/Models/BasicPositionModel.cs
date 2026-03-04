using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("positions", Schema = "basic")]
public class BasicPositionModel : BaseCompanyModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;
    
    public virtual List<BasicEmployeeModel> Employees { get; set; } = null!;
}
