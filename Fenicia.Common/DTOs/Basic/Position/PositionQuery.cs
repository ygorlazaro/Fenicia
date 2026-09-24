using Fenicia.Common;

namespace Fenicia.Common.DTOs.Basic.Position;

/// <summary>
/// Query DTO for paginated position queries.
/// </summary>
/// <param name="page">The page number (default: 1).</param>
/// <param name="perPage">Items per page (default: 10).</param>
/// <param name="query">Search query string.</param>
/// <param name="sort">Sort expression.</param>
/// <param name="filters">Additional filters.</param>
public record PositionQuery(
    int Page = 1,
    int PerPage = 10,
    string? Query = null,
    string? Sort = null,
    Dictionary<string, string>? Filters = null) : PaginationQuery(Page, PerPage);