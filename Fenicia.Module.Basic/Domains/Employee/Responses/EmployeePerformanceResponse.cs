namespace Fenicia.Module.Basic.Domains.Employee.Responses;

/// <summary>
///     Response model containing comprehensive employee performance data.
///     Includes summary statistics, sales by employee, order counts, and top performers.
/// </summary>
public record EmployeePerformanceResponse
{
    /// <summary>Performance summary statistics.</summary>
    public EmployeePerformanceSummaryResponse Summary { get; set; } = new();

    /// <summary>Sales data grouped by employee.</summary>
    public List<EmployeeSalesResponse> SalesByEmployee { get; set; } = [];

    /// <summary>Order counts grouped by employee.</summary>
    public List<EmployeeOrderCountResponse> OrdersByEmployee { get; set; } = [];

    /// <summary>Top performing employees.</summary>
    public List<TopPerformerResponse> TopPerformers { get; set; } = [];
}