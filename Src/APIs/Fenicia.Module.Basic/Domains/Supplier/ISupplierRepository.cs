using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Supplier;

public interface ISupplierRepository : IRepository<SupplierModel>
{
    Task<List<SupplierModel>> GetAllWithDetailsAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    Task<SupplierModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, string>> GetSupplierNamesAsync(IEnumerable<Guid> supplierIds, CancellationToken cancellationToken = default);
}
