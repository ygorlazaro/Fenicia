using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Dashboard.DTOs;

namespace Fenicia.Module.Basic.Domains.Dashboard.Interfaces;

public interface IDashboardService
{
    Task<FinancialDashboardResponse> GetFinancialDashboardAsync(GetFinancialDashboardQuery query, CancellationToken cancellationToken = default);

    Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default);

    Task<decimal> GetTotalCostAsync(CancellationToken cancellationToken = default);

    Task<int> GetTotalOrdersAsync(CancellationToken cancellationToken = default);

    Task<int> GetTotalProductsAsync(CancellationToken cancellationToken = default);

    Task<int> GetTotalEmployeesAsync(CancellationToken cancellationToken = default);

    Task<List<OrderModel>> GetRecentOrdersAsync(int topLimit, CancellationToken cancellationToken = default);

    Task<List<OrderModel>> GetTopCustomerOrdersAsync(CancellationToken cancellationToken = default);

    Task<List<OrderModel>> GetAtRiskOrdersAsync(CancellationToken cancellationToken = default);

    Task<List<OrderModel>> GetEmployeePerformanceOrdersAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    Task<List<EmployeeModel>> GetAllEmployeesAsync(CancellationToken cancellationToken = default);
}