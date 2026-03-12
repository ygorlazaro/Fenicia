namespace Fenicia.Module.Basic.Domains.Employee.Queries;

public record GetEmployeePerformanceQuery(
    int Days = 90,
    int TopLimit = 10);
