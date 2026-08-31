using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Employee;

public interface IEmployeeRepository : IRepository<EmployeeModel>
{
    Task<EmployeeModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<EmployeeModel>> GetAllWithDetailsAsync(int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<EmployeeModel>> GetByPositionIdAsync(Guid positionId, int page = 1, int perPage = 10, CancellationToken cancellationToken = default);
}
