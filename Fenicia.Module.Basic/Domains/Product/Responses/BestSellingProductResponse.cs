namespace Fenicia.Module.Basic.Domains.Product.Responses;

/// <summary>
/// Response record for a best-selling product in performance metrics.
/// </summary>
public record BestSellingProductResponse(
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
    /// Average price per unit.
    /// </summary>
    decimal AveragePrice);
