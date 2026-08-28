using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer;

public class CustomerRepository(DefaultContext context) : Repository<CustomerModel>(context)
{
    public async Task<CustomerModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .Include(c => c.Person)
            .Include(c => c.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .FirstOrDefaultAsync(e => e.Id == id && e.Deleted == null, ct);
    }

    public async Task<IEnumerable<CustomerModel>> GetAllWithDetailsAsync(int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await DbSet
            .Include(c => c.Person)
            .Include(c => c.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .Where(e => e.Deleted == null)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }
}
