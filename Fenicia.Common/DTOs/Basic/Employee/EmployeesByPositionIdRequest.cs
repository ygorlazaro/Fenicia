using System.ComponentModel.DataAnnotations;
using Fenicia.Common;

namespace Fenicia.Common.DTOs.Basic.Employee;

/// <summary>
/// Request DTO for retrieving employees by position ID with pagination.
/// </summary>
/// <param name="positionId">The unique identifier of the position.</param>
/// <param name="page">The page number (default: 1).</param>
/// <param name="perPage">The number of items per page (default: 10).</param>
public record EmployeesByPositionIdRequest(
    [Required] Guid PositionId,
    int Page = 1,
    int PerPage = 10) : PaginationQuery(Page, PerPage)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeesByPositionIdRequest"/> class with default values.
    /// </summary>
    public EmployeesByPositionIdRequest()
        : this(Guid.Empty, 1, 10)
    {
    }
}