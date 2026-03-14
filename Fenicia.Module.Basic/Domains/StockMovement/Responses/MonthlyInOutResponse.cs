namespace Fenicia.Module.Basic.Domains.StockMovement.Responses;

/// <summary>
///     Response record for monthly stock in/out totals.
/// </summary>
public record MonthlyInOutResponse(
    /// <summary>
    /// Month and year (MM/YYYY format).
    /// </summary>
    string Month,
    /// <summary>
    /// Total quantity of stock movements in.
    /// </summary>
    double TotalIn,
    /// <summary>
    /// Total quantity of stock movements out.
    /// </summary>
    double TotalOut,
    /// <summary>
    /// Total value of stock movements in.
    /// </summary>
    decimal TotalInValue,
    /// <summary>
    /// Total value of stock movements out.
    /// </summary>
    decimal TotalOutValue);