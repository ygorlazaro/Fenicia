using Fenicia.Common.Data.Converters.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Supplier;

public class SupplierService(ISupplierRepository supplierRepository) : ISupplierService
{
    public async Task<List<SupplierResponse>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int perPage = 1)
    {
        var suppliers = await supplierRepository.GetAllAsync(cancellationToken, page, perPage);

        return SupplierConverter.Convert(suppliers);
    }

    public async Task<SupplierResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(id, cancellationToken);

        return supplier is null ? null : SupplierConverter.Convert(supplier);
    }

    public async Task<SupplierResponse?> AddAsync(SupplierRequest request, CancellationToken cancellationToken)
    {
        var supplier = SupplierConverter.Convert(request);

        supplierRepository.Add(supplier);

        await supplierRepository.SaveChangesAsync(cancellationToken);

        return SupplierConverter.Convert(supplier);
    }

    public async Task<SupplierResponse?> UpdateAsync(SupplierRequest request, CancellationToken cancellationToken)
    {
        var supplier = SupplierConverter.Convert(request);

        supplierRepository.Update(supplier);

        await supplierRepository.SaveChangesAsync(cancellationToken);

        return SupplierConverter.Convert(supplier);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        supplierRepository.Delete(id);

        await supplierRepository.SaveChangesAsync(cancellationToken);
    }
}
