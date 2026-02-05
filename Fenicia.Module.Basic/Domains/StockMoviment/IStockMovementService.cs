using Fenicia.Common.Database.Requests.Basic;
using Fenicia.Common.Database.Responses.Auth;

namespace Fenicia.Module.Basic.Domains.StockMoviment;

public interface IStockMovementService
{
    Task AddStock(Guid productId, int quantity, CancellationToken cancellationToken);

    Task RemoveStock(Guid productId, int quantity, CancellationToken cancellationToken);

    Task<List<StockMovementResponse>> GetMovementAsync(Guid productId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken, int page = 1, int perPage = 10);

    Task<List<StockMovementResponse>> GetMovementAsync(DateTime queryStartDate, DateTime queryEndDate, CancellationToken cancellationToken, int queryPage = 1, int perPage = 10);

    Task<StockMovementResponse?> AddAsync(StockMovementRequest request, CancellationToken cancellationToken);

    Task<StockMovementResponse?> UpdateAsync(Guid stockMovementId, StockMovementRequest request, CancellationToken cancellationToken);
}
