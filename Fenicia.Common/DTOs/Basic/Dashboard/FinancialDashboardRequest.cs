namespace Fenicia.Common.DTOs.Basic.Dashboard;

/// <summary>
/// Request DTO for retrieving financial dashboard data.
/// </summary>
public class FinancialDashboardRequest()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FinancialDashboardRequest"/> class with the specified period.
    /// </summary>
    /// <param name="days">The period in days for analysis (default: 90).</param>
    public FinancialDashboardRequest(int days = 90)
        : this()
    {
        Days = days;
    }

    /// <summary>
    /// Gets or sets the period in days for analysis.
    /// </summary>
    public int Days { get; set; }
}