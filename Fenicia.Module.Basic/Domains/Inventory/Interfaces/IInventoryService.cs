using Fenicia.Common;
using Fenicia.Common.DTOs.Basic.Inventory;

namespace Fenicia.Module.Basic.Domains.Inventory.Interfaces;

/// <summary>
/// Service interface for inventory operations.
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// Gets the inventory with pagination and filtering.
    /// </summary>
    /// <param name="request">The inventory request containing pagination and filter parameters.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The inventory response.</returns>
    Task<InventoryResponse> GetAsync(
        InventoryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the inventory by category with pagination.
    /// </summary>
    /// <param name="request">The inventory by category request.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The inventory response.</returns>
    Task<InventoryResponse> GetByCategoryAsync(
        InventoryByCategoryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the inventory by product with pagination.
    /// </summary>
    /// <param name="request">The inventory by product request.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The inventory response.</returns>
    Task<InventoryResponse> GetByProductAsync(
        InventoryByProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the inventory dashboard data.
    /// </summary>
    /// <param name="request">The inventory dashboard request.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The inventory dashboard response.</returns>
    Task<InventoryDashboardResponse> GetDashboardAsync(
        InventoryDashboardRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the inventory health data.
    /// </summary>
    /// <param name="request">The inventory health request.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The inventory health response.</returns>
    Task<InventoryHealthResponse> GetHealthAsync(
        InventoryHealthRequest request,
        CancellationToken cancellationToken = default);
}