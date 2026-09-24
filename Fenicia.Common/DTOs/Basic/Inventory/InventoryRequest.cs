using Fenicia.Common;

namespace Fenicia.Common.DTOs.Basic.Inventory;

/// <summary>
/// Request DTO for retrieving inventory with pagination and filtering.
/// </summary>
/// <param name="page">The page number (default: 1).</param>
/// <param name="perPage">The number of items per page (default: 10).</param>
/// <param name="query">The search query (optional).</param>
/// <param name="sort">The sort field (optional).</param>
/// <param name="filters">Additional filters (optional).</param>
public record InventoryRequest(
    int Page = 1,
    int PerPage = 10,
    string? Query = null,
    string? Sort = null,
    Dictionary<string, string>? Filters = null) : PaginationQuery(Page, PerPage)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryRequest"/> class with default values.
    /// </summary>
    public InventoryRequest()
        : this(1, 10, null, null, null)
    {
    }
}