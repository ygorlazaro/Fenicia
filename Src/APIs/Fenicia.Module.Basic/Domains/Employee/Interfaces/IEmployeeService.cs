using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.Employee.DTOs;

namespace Fenicia.Module.Basic.Domains.Employee.Interfaces;

public interface IEmployeeService
{
    Task<Pagination<List<GetAllEmployeeResponse>>> GetAllAsync(
        GetAllEmployeeQuery query,
        CancellationToken cancellationToken = default);

    Task<List<GetAllEmployeeForDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default);

    Task<GetEmployeeByIdResponse?> GetByIdAsync(
        GetEmployeeByIdQuery query,
        CancellationToken cancellationToken = default);

    Task<AddEmployeeResponse> AddAsync(
        AddEmployeeCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<UpdateEmployeeResponse?> UpdateAsync(
        UpdateEmployeeCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteEmployeeCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task<EmployeePerformanceResponse> GetPerformanceAsync(
        GetEmployeePerformanceQuery query,
        CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken cancellationToken = default);

    Task<int> GetTotalEmployeesAsync(CancellationToken cancellationToken = default);

    Task<Pagination<List<GetEmployeesByPositionIdResponse>>> GetByPositionIdAsync(
        GetEmployeesByPositionIdQuery query,
        CancellationToken cancellationToken = default);
}