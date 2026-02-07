using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Inventory;

public interface IInventoryService
{
    Task<InventoryResponse> GetInventoryByProductAsync(
        Guid productId,
        CancellationToken ct,
        int page = 1,
        int perPage = 10);

    Task<InventoryResponse> GetInventoryByCategoryAsync(
        Guid categoryId,
        CancellationToken ct,
        int page = 1,
        int perPage = 10);

    Task<InventoryResponse> GetInventoryAsync(CancellationToken ct, int page = 1, int perPage = 10);
}