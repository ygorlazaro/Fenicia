using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Employee.Interfaces;

/// <summary>
/// Repository interface for employee data access.
/// </summary>
public interface IEmployeeRepository : IRepository<EmployeeModel>
{
    /// <summary>
    /// Gets an employee by ID with details.
    /// </summary>
    /// <param name="id">The employee ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The employee model if found, or null.</returns>
    Task<EmployeeModel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all employees with details using pagination.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="perPage">The number of items per page.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A collection of employee models.</returns>
    Task<IEnumerable<EmployeeModel>> GetAllWithDetailsAsync(
        int page,
        int perPage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all employees for data source.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of employee models.</returns>
    Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets employees by position ID with pagination.
    /// </summary>
    /// <param name="positionId">The position ID.</param>
    /// <param name="page">The page number.</param>
    /// <param name="perPage">The number of items per page.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A collection of employee models.</returns>
    Task<IEnumerable<EmployeeModel>> GetByPositionIdAsync(
        Guid positionId,
        int page,
        int perPage,
        CancellationToken cancellationToken = default);
}