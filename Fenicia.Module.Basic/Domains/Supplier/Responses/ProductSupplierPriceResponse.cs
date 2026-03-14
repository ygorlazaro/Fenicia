namespace Fenicia.Module.Basic.Domains.Supplier.Responses;

/// <summary>
///     Response record for a supplier's price for a specific product.
/// </summary>
public record ProductSupplierPriceResponse(
    /// <summary>
    /// Supplier ID.
    /// </summary>
    Guid SupplierId,
    /// <summary>
    /// Supplier name.
    /// </summary>
    string SupplierName,
    /// <summary>
    /// Cost price from this supplier.
    /// </summary>
    decimal CostPrice,
    /// <summary>
    /// Sales price.
    /// </summary>
    decimal SalesPrice,
    /// <summary>
    /// Profit margin percentage.
    /// </summary>
    decimal ProfitMargin);