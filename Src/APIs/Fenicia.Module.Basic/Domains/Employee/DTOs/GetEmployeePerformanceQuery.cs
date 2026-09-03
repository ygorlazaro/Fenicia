namespace Fenicia.Module.Basic.Domains.Employee.DTOs;

public record GetEmployeePerformanceQuery(int Days = 90, int TopLimit = 10);