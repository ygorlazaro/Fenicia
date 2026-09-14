namespace Fenicia.Common.DTOs.Basic.Employee;

public record GetEmployeePerformanceQuery(int Days = 90, int TopLimit = 10);