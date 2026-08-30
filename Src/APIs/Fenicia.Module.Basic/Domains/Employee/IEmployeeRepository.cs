using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Employee;

public interface IEmployeeRepository : IRepository<EmployeeModel>
{
    Task<EmployeeModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);

    Task<IEnumerable<EmployeeModel>> GetAllWithDetailsAsync(int page = 1, int perPage = 10, CancellationToken ct = default);

    Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken ct = default);

    Task<IEnumerable<EmployeeModel>> GetByPositionIdAsync(Guid positionId, int page = 1, int perPage = 10, CancellationToken ct = default);
}