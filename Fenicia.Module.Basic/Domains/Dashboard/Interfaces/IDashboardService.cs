using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Dashboard;
using Fenicia.Common.DTOs.Basic.Order;

namespace Fenicia.Module.Basic.Domains.Dashboard.Interfaces;

/// <summary>
/// Interface for dashboard service operations.
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Gets the financial dashboard data based on the specified request.
    /// </summary>
    /// <param name="request">The financial dashboard request containing filter parameters.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The financial dashboard response.</returns>
    Task<FinancialDashboardResponse> GetFinancialDashboardAsync(
        FinancialDashboardRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total cost.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total cost.</returns>
    Task<decimal> GetTotalCostAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total number of products.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total number of products.</returns>
    Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total number of employees.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total number of employees.</returns>
    Task<int> GetTotalEmployeesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the recent orders.
    /// </summary>
    /// <param name="topLimit">The maximum number of orders to return.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order responses.</returns>
    Task<List<OrderResponse>> GetRecentOrdersAsync(int topLimit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the top customer orders.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order responses.</returns>
    Task<List<OrderResponse>> GetTopCustomerOrdersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the at-risk orders.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order responses.</returns>
    Task<List<OrderResponse>> GetAtRiskOrdersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the employee performance orders within the specified date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of order responses.</returns>
    Task<List<OrderResponse>> GetEmployeePerformanceOrdersAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all employees.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of employee models.</returns>
    Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken cancellationToken = default);
}