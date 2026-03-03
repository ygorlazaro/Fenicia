using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("positions", Schema = "basic")]
public class BasicPosition : BaseCompanyModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;
    public virtual List<BasicEmployee> Employees { get; set; } = null!;
}
