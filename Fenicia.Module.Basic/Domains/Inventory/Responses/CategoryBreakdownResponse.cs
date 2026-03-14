namespace Fenicia.Module.Basic.Domains.Inventory.Responses;

/// <summary>
///     Response model for inventory breakdown by category.
/// </summary>
public record CategoryBreakdownResponse(
    /// <summary>Category ID.</summary>
    Guid CategoryId,
    /// <summary>Category name.</summary>
    string CategoryName,
    /// <summary>Total cost value for the category.</summary>
    decimal TotalCostValue,
    /// <summary>Total sales value for the category.</summary>
    decimal TotalSalesValue,
    /// <summary>Total quantity in the category.</summary>
    double TotalQuantity);