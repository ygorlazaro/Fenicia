using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Requests.Basic;

namespace Fenicia.Common.Data.Models.Basic;

[Table("employees")]
public sealed class EmployeeModel : BaseModel
{
    public EmployeeModel(EmployeeRequest request)
    {
        this.Id = request.Id;
        this.PositionId = request.PositionId;
        this.Person = new PersonModel(request.Person);
        this.PersonId = this.Person.Id;

        this.Position = null!;
    }

    [Required]
    public Guid PositionId { get; set; }

    [ForeignKey(nameof(PositionId))]
    public PositionModel Position { get; set; }

    public Guid PersonId { get; set; }

    public PersonModel Person { get; set; }
}
