using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Employee;

public interface IEmployeeService
{
    Task<List<EmployeeResponse>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int perPage = 1);

    Task<EmployeeResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<EmployeeResponse?> AddAsync(EmployeeRequest request, CancellationToken cancellationToken);

    Task<EmployeeResponse?> UpdateAsync(EmployeeRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<List<EmployeeResponse>> GetByPositionIdAsync(Guid id, CancellationToken cancellationToken, int page = 1, int perPage = 10);
}
