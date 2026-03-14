namespace Fenicia.Module.Basic.Domains.Dashboard.Responses;

/// <summary>
///     Response model for revenue vs cost analysis.
///     Compares revenue, cost, and profit for a specific period.
/// </summary>
public record RevenueVsCostResponse(
    /// <summary>Period name (e.g., "2024 March 15").</summary>
    string Period,
    /// <summary>Date of the period.</summary>
    DateTime Date,
    /// <summary>Total revenue for the period.</summary>
    decimal Revenue,
    /// <summary>Total cost for the period.</summary>
    decimal Cost,
    /// <summary>Total profit (revenue - cost) for the period.</summary>
    decimal Profit);