using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Module.Basic.Domains.Employee;

public interface IEmployeeService
{
    Task<List<EmployeeResponse>> GetAllAsync(CancellationToken ct, int page = 1, int perPage = 1);

    Task<EmployeeResponse?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<EmployeeResponse?> AddAsync(EmployeeRequest request, CancellationToken ct);

    Task<EmployeeResponse?> UpdateAsync(EmployeeRequest request, CancellationToken ct);

    Task DeleteAsync(Guid id, CancellationToken ct);

    Task<List<EmployeeResponse>> GetByPositionIdAsync(Guid id, CancellationToken ct, int page = 1, int perPage = 10);
}