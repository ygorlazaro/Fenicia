namespace Fenicia.Module.Basic.Domains.Employee.Responses;

/// <summary>
/// Response model for top performing employees.
/// Contains performance metrics and performance level rating.
/// </summary>
public record TopPerformerResponse(
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
    /// <summary>Performance level (Standard, Good, Very Good, Excellent).</summary>
    string PerformanceLevel);
