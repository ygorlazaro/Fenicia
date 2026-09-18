using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

public class UpdateEmployeeResponse()
{
    public UpdateEmployeeResponse(Guid id, Guid positionId, Guid personId)
        : this()
    {
        Id = id;
        PositionId = positionId;
        PersonId = personId;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid PositionId { get; set; }

    [Required]
    public Guid PersonId { get; set; }
}
