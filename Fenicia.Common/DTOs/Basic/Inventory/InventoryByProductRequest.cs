using System.ComponentModel.DataAnnotations;
using Fenicia.Common;

namespace Fenicia.Common.DTOs.Basic.Inventory;

/// <summary>
/// Request DTO for retrieving inventory by product with pagination.
/// </summary>
/// <param name="productId">The unique identifier of the product.</param>
/// <param name="page">The page number (default: 1).</param>
/// <param name="perPage">The number of items per page (default: 10).</param>
public record InventoryByProductRequest(
    [Required] Guid ProductId,
    int Page = 1,
    int PerPage = 10) : PaginationQuery(Page, PerPage)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryByProductRequest"/> class with default values.
    /// </summary>
    public InventoryByProductRequest()
        : this(Guid.Empty, 1, 10)
    {
    }
}