using Fenicia.Common;
using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Common.DTOs.Basic.Inventory;
using Fenicia.Common.DTOs.Basic.Supplier;
using SupplierResponse = Fenicia.Common.DTOs.Basic.Supplier.GetAllSupplierResponse;
using SupplierRequest = Fenicia.Common.DTOs.Basic.Supplier.AddSupplierCommand;
using SupplierQuery = Fenicia.Common.DTOs.Basic.Supplier.GetAllSupplierQuery;
using SupplierPerformanceQuery = Fenicia.Common.DTOs.Basic.Supplier.GetSupplierPerformanceQuery;
using GetAllSupplierForDataSourceResponse = Fenicia.Common.DTOs.Basic.DataSource.SupplierDataSourceResponse;

/// <summary>
/// Fenicia.Module.Basic/Domains/Supplier/Interfaces/ISupplierService.cs implementation.
/// </summary>

namespace Fenicia.Module.Basic.Domains.Supplier.Interfaces;

public interface ISupplierService
{
    Task<Pagination<List<SupplierResponse>>> GetAllAsync(
        SupplierQuery query,
        CancellationToken cancellationToken = default);

    Task<List<SupplierDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default);

    Task<SupplierResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SupplierResponse> AddAsync(
        SupplierRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<SupplierResponse?> UpdateAsync(
        Guid id,
        SupplierRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, Guid companyId, CancellationToken cancellationToken = default);

    Task<SupplierPerformanceResponse> GetPerformanceAsync(
        SupplierPerformanceQuery query,
        CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<List<SupplierProductCountResponse>> GetProductStatsAsync(CancellationToken cancellationToken = default);

    Task<List<SupplierStockMovementResponse>> GetRecentStockMovementsAsync(
        int days,
        int topLimit,
        CancellationToken cancellationToken = default);

    Task<List<SupplierCostComparisonResponse>> GetCostComparisonAsync(
        int topLimit,
        CancellationToken cancellationToken = default);

    Task<List<SupplierBreakdownResponse>> GetSupplierBreakdownAsync(CancellationToken cancellationToken = default);

    Task<List<SupplierResponse>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);
}