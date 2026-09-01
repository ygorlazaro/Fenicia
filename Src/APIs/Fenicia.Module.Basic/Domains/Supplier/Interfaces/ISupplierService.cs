using Fenicia.Common;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;

namespace Fenicia.Module.Basic.Domains.Supplier.Interfaces;

public interface ISupplierService
{
    Task<Pagination<List<GetAllSupplierResponse>>> GetAllAsync(GetAllSupplierQuery query, CancellationToken cancellationToken = default);

    Task<List<GetAllSupplierForDataSourceResponse>> GetAllForDataSourceAsync(CancellationToken cancellationToken = default);

    Task<GetSupplierByIdResponse?> GetByIdAsync(GetSupplierByIdQuery query, CancellationToken cancellationToken = default);

    Task<AddSupplierResponse> AddAsync(AddSupplierCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task<UpdateSupplierResponse?> UpdateAsync(UpdateSupplierCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteSupplierCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task<SupplierPerformanceResponse> GetPerformanceAsync(GetSupplierPerformanceQuery query, CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<List<SupplierProductCountResponse>> GetProductStatsAsync(CancellationToken cancellationToken = default);

    Task<List<SupplierStockMovementResponse>> GetRecentStockMovementsAsync(int days, int topLimit, CancellationToken cancellationToken = default);

    Task<List<SupplierCostComparisonResponse>> GetCostComparisonAsync(int topLimit, CancellationToken cancellationToken = default);

    Task<List<SupplierBreakdownResponse>> GetSupplierBreakdownAsync(CancellationToken cancellationToken = default);

    Task<List<GetSupplierByIdResponse>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
