namespace Fenicia.Module.Basic.Domains.Inventory.Responses;

/// <summary>
///     Response model for stock value by category.
/// </summary>
public record StockValueByCategoryResponse(
    /// <summary>Category ID.</summary>
    Guid CategoryId,
    /// <summary>Category name.</summary>
    string CategoryName,
    /// <summary>Number of products in the category.</summary>
    int ProductCount,
    /// <summary>Total stock value for the category.</summary>
    decimal TotalStockValue,
    /// <summary>Percentage of total stock value.</summary>
    double Percentage);