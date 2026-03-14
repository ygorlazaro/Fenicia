namespace Fenicia.Module.Basic.Domains.Employee.Responses;

/// <summary>
///     Response model for employee sales data.
///     Contains sales metrics for a specific employee.
/// </summary>
public record EmployeeSalesResponse(
    /// <summary>Employee ID.</summary>
    Guid EmployeeId,
    /// <summary>Employee name.</summary>
    string EmployeeName,
    /// <summary>Employee position name.</summary>
    string PositionName,
    /// <summary>Total sales amount.</summary>
    decimal TotalSales,
    /// <summary>Total number of orders.</summary>
    int TotalOrders,
    /// <summary>Average order value.</summary>
    decimal AverageOrderValue,
    /// <summary>Sales rank among employees.</summary>
    int Rank);