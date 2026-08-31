using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.StockMovement;

public interface IStockMovementRepository : IRepository<StockMovementModel>
{
    Task<IEnumerable<StockMovementModel>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, DateTime?>> GetLastMovementsByProductIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default);

    Task<IEnumerable<StockMovementModel>> GetWithDetailsAsync(DateTime startDate, DateTime endDate, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    Task<IEnumerable<StockMovementModel>> GetWithDetailsForDashboardAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    Task<IEnumerable<StockMovementModel>> GetByDateRangeAsync(DateTime startDate, CancellationToken cancellationToken = default);
}
