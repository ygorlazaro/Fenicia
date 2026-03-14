namespace Fenicia.Module.Basic.Domains.Inventory.Responses;

/// <summary>
///     Response model for overstock product details.
/// </summary>
public record OverstockProductResponse(
    /// <summary>Product ID.</summary>
    Guid ProductId,
    /// <summary>Product name.</summary>
    string ProductName,
    /// <summary>Category name.</summary>
    string CategoryName,
    /// <summary>Current stock quantity.</summary>
    double CurrentQuantity,
    /// <summary>Recommended quantity based on sales.</summary>
    double RecommendedQuantity,
    /// <summary>Excess value of overstock.</summary>
    decimal ExcessValue,
    /// <summary>Cost price per unit.</summary>
    decimal CostPrice);