using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

public class GetEmployeesByPositionIdQuery()
{
    public GetEmployeesByPositionIdQuery(Guid positionId, int page = 1, int perPage = 10)
        : this()
    {
        PositionId = positionId;
        Page = page;
        PerPage = perPage;
    }

    [Required]
    public Guid PositionId { get; set; }

    public int Page { get; set; }

    public int PerPage { get; set; }
}
