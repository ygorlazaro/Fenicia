namespace Fenicia.Module.Basic.Domains.Employee.GetEmployeePerformance;

public record GetEmployeePerformanceQuery(
    int Days = 90,
    int TopLimit = 10);
