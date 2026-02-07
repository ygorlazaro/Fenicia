using Fenicia.Common.Data.Mappers.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Supplier;

public class SupplierService(ISupplierRepository supplierRepository) : ISupplierService
{
    public async Task<List<SupplierResponse>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 1)
    {
        var suppliers = await supplierRepository.GetAllAsync(ct, page, perPage);

        return SupplierMapper.Map(suppliers);
    }

    public async Task<SupplierResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var supplier = await supplierRepository.GetByIdAsync(id, ct);

        return supplier is null ? null : SupplierMapper.Map(supplier);
    }

    public async Task<SupplierResponse?> AddAsync(SupplierRequest request, CancellationToken ct)
    {
        var supplier = SupplierMapper.Map(request);

        supplierRepository.Add(supplier);

        await supplierRepository.SaveChangesAsync(ct);

        return SupplierMapper.Map(supplier);
    }

    public async Task<SupplierResponse?> UpdateAsync(SupplierRequest request, CancellationToken ct)
    {
        var supplier = SupplierMapper.Map(request);

        supplierRepository.Update(supplier);

        await supplierRepository.SaveChangesAsync(ct);

        return SupplierMapper.Map(supplier);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        supplierRepository.Delete(id);

        await supplierRepository.SaveChangesAsync(ct);
    }
}
