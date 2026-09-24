using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.Basic;

[Table("employees", Schema = "basic")]
public class EmployeeModel : BaseCompanyModel
{
    [Required]
    public Guid PositionId { get; set; }

    [ForeignKey(nameof(PositionId))]
    public PositionModel Position { get; init; } = null!;

    public Guid PersonId { get; init; }

    public PersonModel Person { get; init; } = null!;
}
