namespace Fenicia.Module.Basic.Domains.Product.Commands;

/// <summary>
/// Command record for creating a new product.
/// </summary>
public record AddProductCommand(
    /// <summary>
    /// Unique identifier for the new product.
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
    /// Supplier ID (optional).
    /// </summary>
    Guid? SupplierId);
