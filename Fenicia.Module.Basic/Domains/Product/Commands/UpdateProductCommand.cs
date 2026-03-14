namespace Fenicia.Module.Basic.Domains.Product.Commands;

/// <summary>
///     Command record for updating an existing product.
/// </summary>
public record UpdateProductCommand(
    /// <summary>
    /// Unique identifier of the product to update.
    /// </summary>
    Guid Id,
    /// <summary>
    /// Updated name of the product.
    /// </summary>
    string Name,
    /// <summary>
    /// Updated cost price of the product.
    /// </summary>
    decimal? CostPrice,
    /// <summary>
    /// Updated sales price of the product.
    /// </summary>
    decimal SalesPrice,
    /// <summary>
    /// Updated quantity in stock.
    /// </summary>
    double Quantity,
    /// <summary>
    /// Updated category ID.
    /// </summary>
    Guid CategoryId,
    /// <summary>
    /// Updated supplier ID (optional).
    /// </summary>
    Guid? SupplierId);