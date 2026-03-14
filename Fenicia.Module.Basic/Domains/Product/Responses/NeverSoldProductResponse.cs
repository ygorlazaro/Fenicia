namespace Fenicia.Module.Basic.Domains.Product.Responses;

/// <summary>
///     Response record for products that have never been sold.
/// </summary>
public record NeverSoldProductResponse(
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
    /// Supplier name (optional).
    /// </summary>
    string? SupplierName,
    /// <summary>
    /// Current stock quantity.
    /// </summary>
    double CurrentStock,
    /// <summary>
    /// Total cost value of current stock.
    /// </summary>
    decimal CostValue,
    /// <summary>
    /// Date of last stock movement (optional).
    /// </summary>
    DateTime? LastStockMovement);