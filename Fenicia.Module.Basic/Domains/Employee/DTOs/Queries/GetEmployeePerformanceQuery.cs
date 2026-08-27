using Fenicia.Module.Basic.Domains.Employee.DTOs.Responses;


namespace Fenicia.Module.Basic.Domains.Employee.DTOs.Queries;

public record GetEmployeePerformanceQuery(int Days = 90, int TopLimit = 10);
