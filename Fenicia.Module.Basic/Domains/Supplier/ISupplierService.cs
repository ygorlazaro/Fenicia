using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Supplier;

public interface ISupplierService
{
    Task<List<SupplierResponse>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int perPage = 1);

    Task<SupplierResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<SupplierResponse?> AddAsync(SupplierRequest request, CancellationToken cancellationToken);

    Task<SupplierResponse?> UpdateAsync(SupplierRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
