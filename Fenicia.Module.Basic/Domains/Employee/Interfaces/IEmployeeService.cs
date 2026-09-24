using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Common.DTOs.Basic.Employee;

namespace Fenicia.Module.Basic.Domains.Employee.Interfaces;

public interface IEmployeeService
{
    Task<Pagination<List<EmployeeResponse>>> GetAllAsync(
        EmployeeQuery query,
        CancellationToken cancellationToken = default);

    Task<List<EmployeeDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default);

    Task<EmployeeResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<EmployeeResponse> AddAsync(
        EmployeeRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<EmployeeResponse?> UpdateAsync(
        Guid id,
        EmployeeRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, Guid companyId, CancellationToken cancellationToken = default);

    Task<EmployeePerformanceResponse> GetPerformanceAsync(
        EmployeePerformanceQuery query,
        CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken cancellationToken = default);

    Task<int> GetTotalEmployeesAsync(CancellationToken cancellationToken = default);

    Task<Pagination<List<EmployeeResponse>>> GetByPositionIdAsync(
        EmployeesByPositionIdQuery query,
        CancellationToken cancellationToken = default);
}