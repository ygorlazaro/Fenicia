namespace Fenicia.Module.Basic.Domains.Supplier.Responses;

/// <summary>
/// Response record for cost comparison of products with multiple suppliers.
/// </summary>
public record SupplierCostComparisonResponse(
    /// <summary>
    /// Product name.
    /// </summary>
    string ProductName,
    /// <summary>
    /// List of suppliers with their prices for this product.
    /// </summary>
    List<ProductSupplierPriceResponse> Suppliers);
