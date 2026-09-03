using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("positions", Schema = "basic")]
public sealed class PositionModel : BaseCompanyModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public List<EmployeeModel> Employees { get; init; } = [];
}