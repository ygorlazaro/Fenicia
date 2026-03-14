namespace Fenicia.Module.Basic.Domains.Employee.Responses;

/// <summary>
///     Response model containing aggregate employee performance statistics.
/// </summary>
public record EmployeePerformanceSummaryResponse
{
    /// <summary>Total number of employees in the system.</summary>
    public int TotalEmployees { get; set; }

    /// <summary>Number of employees with orders in the period.</summary>
    public int ActiveEmployees { get; set; }

    /// <summary>Total sales amount.</summary>
    public decimal TotalSales { get; set; }

    /// <summary>Total number of orders.</summary>
    public int TotalOrders { get; set; }

    /// <summary>Average sales per employee.</summary>
    public decimal AverageSalesPerEmployee { get; set; }

    /// <summary>Average orders per employee.</summary>
    public decimal AverageOrdersPerEmployee { get; set; }
}