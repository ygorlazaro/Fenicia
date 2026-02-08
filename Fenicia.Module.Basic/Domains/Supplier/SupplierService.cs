using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Supplier;

public class SupplierService(ISupplierRepository supplierRepository) : ISupplierService
{
    public async Task<List<SupplierResponse>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 1)
    {
        var suppliers = await supplierRepository.GetAllAsync(ct, page, perPage);

        return [..suppliers.Select(s => new SupplierResponse(s))];
    }

    public async Task<SupplierResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var supplier = await supplierRepository.GetByIdAsync(id, ct);

        return supplier is null ? null : new SupplierResponse(supplier);
    }

    public async Task<SupplierResponse?> AddAsync(SupplierRequest request, CancellationToken ct)
    {
        var supplier = new SupplierModel(request);

        supplierRepository.Add(supplier);

        await supplierRepository.SaveChangesAsync(ct);

        return new SupplierResponse(supplier);
    }

    public async Task<SupplierResponse?> UpdateAsync(SupplierRequest request, CancellationToken ct)
    {
        var supplier = new SupplierModel(request);

        supplierRepository.Update(supplier);

        await supplierRepository.SaveChangesAsync(ct);

        return new SupplierResponse(supplier);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await supplierRepository.DeleteAsync(id, ct);

        await supplierRepository.SaveChangesAsync(ct);
    }
}