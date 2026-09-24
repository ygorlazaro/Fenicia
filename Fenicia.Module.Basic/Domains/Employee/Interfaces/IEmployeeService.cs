using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Common.DTOs.Basic.Employee;

namespace Fenicia.Module.Basic.Domains.Employee.Interfaces;

/// <summary>
/// Service interface for employee operations.
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Gets a paginated list of employees.
    /// </summary>
    /// <param name="query">The pagination and filter query.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A paginated list of employee responses.</returns>
    Task<Pagination<List<EmployeeResponse>>> GetAllAsync(
        EmployeeQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all employees for data source (dropdowns).
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of employee data source responses.</returns>
    Task<List<EmployeeDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an employee by ID.
    /// </summary>
    /// <param name="id">The employee ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The employee response if found, or null.</returns>
    Task<EmployeeResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new employee.
    /// </summary>
    /// <param name="command">The employee request data.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The created employee response.</returns>
    Task<EmployeeResponse> AddAsync(
        EmployeeRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing employee.
    /// </summary>
    /// <param name="id">The employee ID.</param>
    /// <param name="command">The employee request data.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The updated employee response if found, or null.</returns>
    Task<EmployeeResponse?> UpdateAsync(
        Guid id,
        EmployeeRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an employee.
    /// </summary>
    /// <param name="id">The employee ID.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id, Guid companyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets employee performance data.
    /// </summary>
    /// <param name="request">The performance request parameters.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The employee performance response.</returns>
    Task<EmployeePerformanceResponse> GetPerformanceAsync(
        EmployeePerformanceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of employees.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total count.</returns>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all employees.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of employee models.</returns>
    Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total number of employees.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total count.</returns>
    Task<int> GetTotalEmployeesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets employees by position ID with pagination.
    /// </summary>
    /// <param name="query">The query containing position ID and pagination.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A paginated list of employee responses.</returns>
    Task<Pagination<List<EmployeeResponse>>> GetByPositionIdAsync(
        EmployeesByPositionIdRequest query,
        CancellationToken cancellationToken = default);
}