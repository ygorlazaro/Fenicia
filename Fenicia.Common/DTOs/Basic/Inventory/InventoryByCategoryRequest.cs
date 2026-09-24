using System.ComponentModel.DataAnnotations;
using Fenicia.Common;

namespace Fenicia.Common.DTOs.Basic.Inventory;

/// <summary>
/// Request DTO for retrieving inventory by category with pagination.
/// </summary>
/// <param name="categoryId">The unique identifier of the category.</param>
/// <param name="page">The page number (default: 1).</param>
/// <param name="perPage">The number of items per page (default: 10).</param>
public record InventoryByCategoryRequest(
    [Required] Guid CategoryId,
    int Page = 1,
    int PerPage = 10) : PaginationQuery(Page, PerPage)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryByCategoryRequest"/> class with default values.
    /// </summary>
    public InventoryByCategoryRequest()
        : this(Guid.Empty, 1, 10)
    {
    }
}