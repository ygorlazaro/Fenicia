using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee;

public class EmployeeRepository(BasicContext context) : BaseRepository<EmployeeModel>(context), IEmployeeRepository
{
    public async Task<List<EmployeeModel>> GetByPositionIdAsync(Guid positionId, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        return await context.Employees.Where(e => e.PositionId == positionId).Skip((page - 1) * perPage).Take(perPage).ToListAsync(cancellationToken);
    }
}
