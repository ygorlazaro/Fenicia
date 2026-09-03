using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;

namespace Fenicia.Module.Basic.Domains.StockMovement.Interfaces;

public interface IStockMovementService
{
    Task<List<GetStockMovementResponse>> GetAsync(
        GetStockMovementQuery query,
        CancellationToken cancellationToken = default);

    Task<AddStockMovementResponse> AddAsync(
        AddStockMovementCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<UpdateStockMovementResponse?> UpdateAsync(
        UpdateStockMovementCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<List<StockMovementModel>> GetRecentWithProductAsync(
        int days,
        int topLimit,
        CancellationToken cancellationToken = default);

    Task<StockMovementDashboardResponse> GetDashboardAsync(
        GetStockMovementDashboardQuery query,
        CancellationToken cancellationToken = default);

    Task<List<StockMovementModel>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, DateTime?>> GetLastMovementsByProductIdsAsync(
        IEnumerable<Guid> productIds,
        CancellationToken cancellationToken = default);
}