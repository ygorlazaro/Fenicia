using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Supplier;

public interface ISupplierRepository : IRepository<SupplierModel>
{
    IQueryable<SupplierModel> Query();

    Task<List<SupplierModel>> GetAllWithDetailsAsync(int page = 1, int perPage = 10, CancellationToken ct);

    Task<SupplierModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct);

    Task<Dictionary<Guid, string>> GetSupplierNamesAsync(IEnumerable<Guid> supplierIds, CancellationToken ct);
}
