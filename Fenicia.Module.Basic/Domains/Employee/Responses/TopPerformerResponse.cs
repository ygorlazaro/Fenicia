namespace Fenicia.Module.Basic.Domains.Employee.Responses;

public record TopPerformerResponse(

    Guid EmployeeId,

    string EmployeeName,

    string PositionName,

    decimal TotalSales,

    int TotalOrders,

    string PerformanceLevel);