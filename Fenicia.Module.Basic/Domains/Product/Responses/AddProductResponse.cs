namespace Fenicia.Module.Basic.Domains.Product.Responses;

/// <summary>
/// Response record for a newly created product.
/// </summary>
public record AddProductResponse(
    /// <summary>
    /// Unique identifier of the product.
    /// </summary>
    Guid Id,
    /// <summary>
    /// Name of the product.
    /// </summary>
    string Name,
    /// <summary>
    /// Cost price of the product.
    /// </summary>
    decimal? CostPrice,
    /// <summary>
    /// Sales price of the product.
    /// </summary>
    decimal SalesPrice,
    /// <summary>
    /// Quantity in stock.
    /// </summary>
    double Quantity,
    /// <summary>
    /// Category ID the product belongs to.
    /// </summary>
    Guid CategoryId,
    /// <summary>
    /// Category name.
    /// </summary>
    string CategoryName,
    /// <summary>
    /// Supplier ID (optional).
    /// </summary>
    Guid? SupplierId,
    /// <summary>
    /// Supplier name (optional).
    /// </summary>
    string? SupplierName);
