using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Module.Basic.Domains.StockMoviment;

public interface IStockMovementService
{
    Task AddStock(Guid productId, int quantity, CancellationToken ct);

    Task RemoveStock(Guid productId, int quantity, CancellationToken ct);

    Task<List<StockMovementResponse>> GetMovementAsync(
        Guid productId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct,
        int page = 1,
        int perPage = 10);

    Task<List<StockMovementResponse>> GetMovementAsync(
        DateTime queryStartDate,
        DateTime queryEndDate,
        CancellationToken ct,
        int queryPage = 1,
        int perPage = 10);

    Task<StockMovementResponse?> AddAsync(StockMovementRequest request, CancellationToken ct);

    Task<StockMovementResponse?> UpdateAsync(Guid stockMovementId, StockMovementRequest request, CancellationToken ct);
}