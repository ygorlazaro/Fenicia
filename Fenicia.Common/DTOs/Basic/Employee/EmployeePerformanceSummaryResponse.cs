namespace Fenicia.Common.DTOs.Basic.Employee;

/// <summary>
/// Response DTO containing employee performance summary metrics.
/// </summary>
public class EmployeePerformanceSummaryResponse
{
    /// <summary>
    /// Gets or sets the total number of employees.
    /// </summary>
    public int TotalEmployees { get; set; }

    /// <summary>
    /// Gets or sets the number of active employees (with orders).
    /// </summary>
    public int ActiveEmployees { get; set; }

    /// <summary>
    /// Gets or sets the total sales amount.
    /// </summary>
    public decimal TotalSales { get; set; }

    /// <summary>
    /// Gets or sets the total number of orders.
    /// </summary>
    public int TotalOrders { get; set; }

    /// <summary>
    /// Gets or sets the average sales per employee.
    /// </summary>
    public decimal AverageSalesPerEmployee { get; init; }

    /// <summary>
    /// Gets or sets the average orders per employee.
    /// </summary>
    public decimal AverageOrdersPerEmployee { get; set; }
}