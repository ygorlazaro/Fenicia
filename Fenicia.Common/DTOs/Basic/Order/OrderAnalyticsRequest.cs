namespace Fenicia.Common.DTOs.Basic.Order;

/// <summary>
/// Request DTO for retrieving order analytics.
/// </summary>
/// <param name="days">The period in days for analysis (default: 90).</param>
/// <param name="topCustomersLimit">The limit of top customers to return (default: 10).</param>
public record OrderAnalyticsRequest(int Days = 90, int TopCustomersLimit = 10);