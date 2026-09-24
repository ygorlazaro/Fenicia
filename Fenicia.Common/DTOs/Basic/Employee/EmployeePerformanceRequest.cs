namespace Fenicia.Common.DTOs.Basic.Employee;

/// <summary>
/// Request DTO for retrieving employee performance data.
/// </summary>
/// <param name="days">The period in days for analysis (default: 90).</param>
/// <param name="topLimit">The limit of top performers to return (default: 10).</param>
public record EmployeePerformanceRequest(int Days = 90, int TopLimit = 10);