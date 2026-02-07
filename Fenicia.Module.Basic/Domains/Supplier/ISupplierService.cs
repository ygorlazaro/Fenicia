using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Supplier;

public interface ISupplierService
{
    Task<List<SupplierResponse>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 1);

    Task<SupplierResponse?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<SupplierResponse?> AddAsync(SupplierRequest request, CancellationToken ct);

    Task<SupplierResponse?> UpdateAsync(SupplierRequest request, CancellationToken ct);

    Task DeleteAsync(Guid id, CancellationToken ct);
}
