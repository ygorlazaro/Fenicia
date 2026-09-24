namespace Fenicia.Common.DTOs.Basic.Dashboard;

/// <summary>
/// Response DTO containing category breakdown data.
/// </summary>
public class CategoryBreakdownResponse
{
    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the revenue amount.
    /// </summary>
    public decimal Revenue { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public double Quantity { get; set; }

    /// <summary>
    /// Gets or sets whether this is an "Other" category aggregation.
    /// </summary>
    public bool IsOther { get; set; }
}