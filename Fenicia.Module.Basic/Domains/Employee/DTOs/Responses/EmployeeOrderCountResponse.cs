namespace Fenicia.Module.Basic.Domains.Employee.DTOs.Responses;

public record EmployeeOrderCountResponse(

    Guid EmployeeId,

    string EmployeeName,

    string PositionName,

    int OrderCount,

    decimal TotalValue,

    DateTime FirstOrderDate,

    DateTime LastOrderDate);