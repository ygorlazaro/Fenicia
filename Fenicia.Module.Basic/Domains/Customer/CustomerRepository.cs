using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Basic.Domains.Customer.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Fenicia.Module.Basic/Domains/Customer/CustomerRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.Basic.Domains.Customer;

public class CustomerRepository(DbContext context) : Repository<CustomerModel>(context), ICustomerRepository
{
    public Task<CustomerModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(c => c.Person)
            .Include(c => c.Person.PersonAddresses)
            .ThenInclude(pa => pa.Address)
            .ThenInclude(a => a.State)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<CustomerModel>> GetAllWithDetailsAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.Person)
            .Include(c => c.Person.PersonAddresses)
            .ThenInclude(pa => pa.Address)
            .ThenInclude(a => a.State)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);
    }
}