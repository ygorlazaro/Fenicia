namespace Fenicia.Module.Basic.Domains.Employee.Queries;

/// <summary>
/// Query record for generating employee performance analytics.
/// </summary>
public record GetEmployeePerformanceQuery(
    int Days = 90,
    int TopLimit = 10);
