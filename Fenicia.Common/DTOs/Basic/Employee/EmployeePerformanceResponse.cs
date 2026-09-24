namespace Fenicia.Common.DTOs.Basic.Employee;

/// <summary>
/// Response DTO containing employee performance analytics.
/// </summary>
public class EmployeePerformanceResponse
{
    /// <summary>
    /// Gets or sets the summary of employee performance metrics.
    /// </summary>
    public EmployeePerformanceSummaryResponse Summary { get; set; } = new();

    /// <summary>
    /// Gets or sets the sales data by employee.
    /// </summary>
    public List<EmployeeSalesResponse> SalesByEmployee { get; set; } = [];

    /// <summary>
    /// Gets or sets the order count data by employee.
    /// </summary>
    public List<EmployeeOrderCountResponse> OrdersByEmployee { get; set; } = [];

    /// <summary>
    /// Gets or sets the top performers.
    /// </summary>
    public List<TopPerformerResponse> TopPerformers { get; set; } = [];
}