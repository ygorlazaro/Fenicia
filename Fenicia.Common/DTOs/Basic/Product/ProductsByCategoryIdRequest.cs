using Fenicia.Common;

namespace Fenicia.Common.DTOs.Basic.Product;

/// <summary>
/// Request DTO for retrieving products by category with pagination.
/// </summary>
/// <param name="categoryId">The category ID.</param>
/// <param name="page">The page number (default: 1).</param>
/// <param name="perPage">The number of items per page (default: 10).</param>
/// <param name="query">The search query (optional).</param>
/// <param name="sort">The sort field (optional).</param>
/// <param name="filters">Additional filters (optional).</param>
public record ProductsByCategoryIdRequest(
    Guid CategoryId,
    int Page = 1,
    int PerPage = 10,
    string? Query = null,
    string? Sort = null,
    Dictionary<string, string>? Filters = null) : PaginationQuery(Page, PerPage);