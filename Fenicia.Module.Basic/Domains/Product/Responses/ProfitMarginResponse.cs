namespace Fenicia.Module.Basic.Domains.Product.Responses;

/// <summary>
/// Response record for profit margin analysis.
/// </summary>
public record ProfitMarginResponse(
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
    /// Cost price of the product.
    /// </summary>
    decimal CostPrice,
    /// <summary>
    /// Sales price of the product.
    /// </summary>
    decimal SalesPrice,
    /// <summary>
    /// Profit margin percentage.
    /// </summary>
    decimal ProfitMargin,
    /// <summary>
    /// Classification of the margin (Excellent, Good, Average, Low, Very Low).
    /// </summary>
    string MarginClassification);