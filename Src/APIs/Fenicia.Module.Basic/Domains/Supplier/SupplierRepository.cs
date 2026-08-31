using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier;

public class SupplierRepository(DefaultContext context) : Repository<SupplierModel>(context), ISupplierRepository
{
    public async Task<List<SupplierModel>> GetAllWithDetailsAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
    {
        return await DbSet
                .Include(s => s.Person)
            .Include(s => s.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    public async Task<SupplierModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
                .Include(s => s.Person)
            .Include(s => s.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Dictionary<Guid, string>> GetSupplierNamesAsync(IEnumerable<Guid> supplierIds, CancellationToken cancellationToken = default)
    {
        var ids = supplierIds.ToList();
        return await DbSet
            .Where(s => ids.Contains(s.Id))
            .Include(s => s.Person)
            .ToDictionaryAsync(s => s.Id, s => s.Person.Name, cancellationToken);
    }
}
