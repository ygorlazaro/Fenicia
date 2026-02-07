using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee;

public class EmployeeRepository(BasicContext context) : BaseRepository<EmployeeModel>(context), IEmployeeRepository
{
    public async Task<List<EmployeeModel>> GetByPositionIdAsync(Guid positionId, CancellationToken ct, int page = 1, int perPage = 10)
    {
        return await context.Employees.Where(e => e.PositionId == positionId).Skip((page - 1) * perPage).Take(perPage).ToListAsync(ct);
    }
}
