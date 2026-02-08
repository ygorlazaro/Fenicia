using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Requests.Basic;

namespace Fenicia.Common.Data.Models.Basic;

[Table("positions")]
public class PositionModel : BaseModel
{
    public PositionModel(PositionRequest request)
    {
        this.Id = request.Id;
        this.Name = request.Name;
    }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    public virtual List<EmployeeModel> Employees { get; set; } = null!;
}