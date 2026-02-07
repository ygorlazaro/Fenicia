using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.StockMoviment;

public interface IStockMovementRepository : IBaseRepository<StockMovementModel>
{
    Task<List<StockMovementModel>> GetMovementAsync(Guid productId, DateTime startDate, DateTime endDate, CancellationToken ct, int page = 1, int perPage = 10);

    Task<List<StockMovementModel>> GetMovementAsync(DateTime startDate, DateTime endDate, CancellationToken ct, int page = 1, int perPage = 10);
}
