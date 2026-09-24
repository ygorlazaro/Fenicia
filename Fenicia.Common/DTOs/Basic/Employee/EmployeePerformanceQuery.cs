namespace Fenicia.Common.DTOs.Basic.Employee;

/// <summary>
/// Query DTO for retrieving employee performance data.
/// </summary>
public class EmployeePerformanceQuery()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeePerformanceQuery"/> class with the specified parameters.
    /// </summary>
    /// <param name="days">The period in days for analysis (default: 90).</param>
    /// <param name="topLimit">The limit of top performers to return (default: 10).</param>
    public EmployeePerformanceQuery(int days = 90, int topLimit = 10)
        : this()
    {
        Days = days;
        TopLimit = topLimit;
    }

    /// <summary>
    /// Gets or sets the period in days for analysis.
    /// </summary>
    public int Days { get; set; }

    /// <summary>
    /// Gets or sets the limit of top performers to return.
    /// </summary>
    public int TopLimit { get; set; }
}