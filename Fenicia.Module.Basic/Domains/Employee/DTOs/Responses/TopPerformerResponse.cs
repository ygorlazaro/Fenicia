namespace Fenicia.Module.Basic.Domains.Employee.DTOs.Responses;

public record TopPerformerResponse(

    Guid EmployeeId,

    string EmployeeName,

    string PositionName,

    decimal TotalSales,

    int TotalOrders,

    string PerformanceLevel);