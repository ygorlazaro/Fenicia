namespace Fenicia.Module.Basic.Domains.Dashboard.Responses;

/// <summary>
///     Response model for profit margin trend analysis.
///     Contains margin percentage and trend direction for a specific period.
/// </summary>
public record ProfitMarginTrendResponse(
    /// <summary>Period name (e.g., "Week 12").</summary>
    string Period,
    /// <summary>Date of the period.</summary>
    DateTime Date,
    /// <summary>Profit margin percentage for this period.</summary>
    decimal MarginPercentage,
    /// <summary>Trend direction (Improving, Stable, Declining).</summary>
    string Trend);