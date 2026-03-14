namespace Fenicia.Module.Basic.Domains.Supplier.Responses;

/// <summary>
///     Response record for supplier product count and value analysis.
/// </summary>
public record SupplierProductCountResponse(
    /// <summary>
    /// Supplier ID.
    /// </summary>
    Guid SupplierId,
    /// <summary>
    /// Supplier name.
    /// </summary>
    string SupplierName,
    /// <summary>
    /// Number of products from this supplier.
    /// </summary>
    int ProductCount,
    /// <summary>
    /// Total stock value.
    /// </summary>
    decimal TotalStockValue,
    /// <summary>
    /// Total potential revenue.
    /// </summary>
    decimal TotalRevenue);