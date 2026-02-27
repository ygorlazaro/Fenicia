using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("employees", Schema = "basic")]
public class BasicEmployee : BaseModel
{
    [Required]
    public Guid PositionId { get; set; }

    [ForeignKey(nameof(PositionId))]
    public BasicPosition Position { get; set; } = null!;

    public Guid PersonId { get; set; }

    public BasicPerson Person { get; set; } = null!;
}
