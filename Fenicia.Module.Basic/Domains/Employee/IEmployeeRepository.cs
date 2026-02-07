using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Employee;

public interface IEmployeeRepository : IBaseRepository<EmployeeModel>
{
    Task<List<EmployeeModel>> GetByPositionIdAsync(Guid positionId, CancellationToken ct, int page = 1, int perPage = 10);
}