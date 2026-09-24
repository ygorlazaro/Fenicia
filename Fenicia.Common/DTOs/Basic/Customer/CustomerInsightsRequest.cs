namespace Fenicia.Common.DTOs.Basic.Customer;

/// <summary>
/// Request DTO for retrieving customer insights.
/// </summary>
public class CustomerInsightsRequest()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerInsightsRequest"/> class with the specified parameters.
    /// </summary>
    /// <param name="days">The period in days for analysis (default: 90).</param>
    /// <param name="topLimit">The limit of records in the top results (default: 10).</param>
    /// <param name="riskThresholdDays">The threshold in days to consider a customer at risk (default: 60).</param>
    public CustomerInsightsRequest(int days = 90, int topLimit = 10, int riskThresholdDays = 60)
        : this()
    {
        Days = days;
        TopLimit = topLimit;
        RiskThresholdDays = riskThresholdDays;
    }

    /// <summary>
    /// Gets or sets the period in days for analysis.
    /// </summary>
    public int Days { get; set; }

    /// <summary>
    /// Gets or sets the limit of records in the top results.
    /// </summary>
    public int TopLimit { get; set; }

    /// <summary>
    /// Gets or sets the threshold in days to consider a customer at risk.
    /// </summary>
    public int RiskThresholdDays { get; set; }
}