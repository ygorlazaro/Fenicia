namespace Fenicia.Module.Basic.Domains.Employee.DTOs.Responses;

public record EmployeeSalesResponse(

    Guid EmployeeId,

    string EmployeeName,

    string PositionName,

    decimal TotalSales,

    int TotalOrders,

    decimal AverageOrderValue,

    int Rank);