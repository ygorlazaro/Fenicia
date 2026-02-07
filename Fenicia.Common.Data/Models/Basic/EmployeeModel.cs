using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("employees")]
public class EmployeeModel : BaseModel
{
    [Required]
    public Guid PositionId { get; set; }

    [ForeignKey(nameof(PositionId))]
    public virtual PositionModel Position { get; set; }

        = null!;

    public Guid PersonId { get; set; }

    public virtual PersonModel Person { get; set; }
}