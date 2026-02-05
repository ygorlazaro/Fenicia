using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Employee;

public interface IEmployeeRepository : IBaseRepository<EmployeeModel>
{
    Task<List<EmployeeModel>> GetByPositionIdAsync(Guid positionId, CancellationToken cancellationToken, int page = 1, int perPage = 10);
}
