using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

/// <summary>
/// Query DTO for retrieving employees by position ID.
/// </summary>
public class EmployeesByPositionIdQuery()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeesByPositionIdQuery"/> class with the specified parameters.
    /// </summary>
    /// <param name="positionId">The unique identifier of the position.</param>
    /// <param name="page">The page number (default: 1).</param>
    /// <param name="perPage">The number of items per page (default: 10).</param>
    public EmployeesByPositionIdQuery(Guid positionId, int page = 1, int perPage = 10)
        : this()
    {
        PositionId = positionId;
        Page = page;
        PerPage = perPage;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the position.
    /// </summary>
    [Required]
    public Guid PositionId { get; set; }

    /// <summary>
    /// Gets or sets the page number.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PerPage { get; set; }
}