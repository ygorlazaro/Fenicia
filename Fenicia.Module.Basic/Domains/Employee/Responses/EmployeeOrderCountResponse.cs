namespace Fenicia.Module.Basic.Domains.Employee.Responses;

/// <summary>
///     Response model for employee order count data.
///     Contains order statistics for a specific employee.
/// </summary>
public record EmployeeOrderCountResponse(
    /// <summary>Employee ID.</summary>
    Guid EmployeeId,
    /// <summary>Employee name.</summary>
    string EmployeeName,
    /// <summary>Employee position name.</summary>
    string PositionName,
    /// <summary>Number of orders.</summary>
    int OrderCount,
    /// <summary>Total order value.</summary>
    decimal TotalValue,
    /// <summary>Date of first order.</summary>
    DateTime FirstOrderDate,
    /// <summary>Date of last order.</summary>
    DateTime LastOrderDate);