namespace Fenicia.Module.Basic.Domains.Employee.GetEmployeePerformance;

public record EmployeeSalesResponse(
    Guid EmployeeId,
    string EmployeeName,
    string PositionName,
    decimal TotalSales,
    int TotalOrders,
    decimal AverageOrderValue,
    int Rank);
