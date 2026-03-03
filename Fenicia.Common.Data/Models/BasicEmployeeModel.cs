using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("employees", Schema = "basic")]
public class BasicEmployeeModel : BaseCompanyModel
{
    [Required]
    public Guid PositionId { get; set; }

    [ForeignKey(nameof(PositionId))]
    public BasicPositionModel PositionModel { get; set; } = null!;

    public Guid PersonId { get; set; }

    public BasicPersonModel PersonModel { get; set; } = null!;
}
