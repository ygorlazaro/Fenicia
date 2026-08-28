using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee;

public class EmployeeRepository(DefaultContext context) : Repository<EmployeeModel>(context)
{
    public async Task<EmployeeModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .Include(e => e.Person)
            .Include(e => e.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == id && e.Deleted == null, ct);
    }

    public async Task<IEnumerable<EmployeeModel>> GetAllWithDetailsAsync(int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await DbSet
            .Include(e => e.Person)
            .Include(e => e.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .Include(e => e.Position)
            .Where(e => e.Deleted == null)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<EmployeeModel>> GetByPositionIdAsync(Guid positionId, int page = 1, int perPage = 10, CancellationToken ct = default)
    {
        return await DbSet
            .Where(e => e.PositionId == positionId && e.Deleted == null)
            .Include(e => e.Person)
            .Include(e => e.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .Include(e => e.Position)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }
}
