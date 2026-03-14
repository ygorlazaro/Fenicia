namespace Fenicia.Module.Basic.Domains.Product.Responses;

/// <summary>
///     Response record for a worst-selling product in performance metrics.
/// </summary>
public record WorstSellingProductResponse(
    /// <summary>
    /// Unique identifier of the product.
    /// </summary>
    Guid ProductId,
    /// <summary>
    /// Name of the product.
    /// </summary>
    string ProductName,
    /// <summary>
    /// Category name.
    /// </summary>
    string CategoryName,
    /// <summary>
    /// Total quantity sold.
    /// </summary>
    double TotalQuantitySold,
    /// <summary>
    /// Total revenue generated.
    /// </summary>
    decimal TotalRevenue,
    /// <summary>
    /// Number of orders containing this product.
    /// </summary>
    int OrderCount,
    /// <summary>
    /// Current stock quantity.
    /// </summary>
    double CurrentStock,
    /// <summary>
    /// Total cost value of current stock.
    /// </summary>
    decimal CostValue);