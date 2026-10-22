using Fenicia.Common;
using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Common.DTOs.Basic.Inventory;
using Fenicia.Common.DTOs.Basic.Supplier;

namespace Fenicia.Module.Basic.Domains.Supplier.Interfaces;

public interface ISupplierService
{
    Task<Pagination<List<GetAllSupplierResponse>>> GetAllAsync(
        GetAllSupplierQuery query,
        CancellationToken cancellationToken = default);

    Task<List<SupplierDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default);

    Task<GetSupplierByIdResponse?> GetByIdAsync(
        GetSupplierByIdQuery query,
        CancellationToken cancellationToken = default);

    Task<AddSupplierResponse> AddAsync(
        AddSupplierRequest request,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<UpdateSupplierResponse?> UpdateAsync(
        Guid id,
        UpdateSupplierRequest request,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, Guid companyId, CancellationToken cancellationToken = default);

    Task<SupplierPerformanceResponse> GetPerformanceAsync(
        GetSupplierPerformanceQuery query,
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

    Task<List<GetAllSupplierResponse>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);
}