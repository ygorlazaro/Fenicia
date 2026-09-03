using Fenicia.Module.Basic.Domains.Inventory.DTOs;

namespace Fenicia.Module.Basic.Domains.Inventory.Interfaces;

public interface IInventoryService
{
    Task<InventoryResponse> GetAsync(GetInventoryQuery query, CancellationToken cancellationToken = default);

    Task<InventoryResponse> GetByCategoryAsync(
        GetInventoryByCategoryQuery query,
        CancellationToken cancellationToken = default);

    Task<InventoryResponse> GetByProductAsync(
        GetInventoryByProductQuery query,
        CancellationToken cancellationToken = default);

    Task<InventoryDashboardResponse> GetDashboardAsync(
        GetInventoryDashboardQuery query,
        CancellationToken cancellationToken = default);

    Task<InventoryHealthResponse> GetHealthAsync(
        GetInventoryHealthQuery query,
        CancellationToken cancellationToken = default);
}