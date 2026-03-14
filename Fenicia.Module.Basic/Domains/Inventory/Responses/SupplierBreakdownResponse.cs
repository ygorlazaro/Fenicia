namespace Fenicia.Module.Basic.Domains.Inventory.Responses;

/// <summary>
///     Response model for inventory breakdown by supplier.
/// </summary>
public record SupplierBreakdownResponse(
    /// <summary>Supplier ID.</summary>
    Guid SupplierId,
    /// <summary>Supplier name.</summary>
    string SupplierName,
    /// <summary>Total cost value from this supplier.</summary>
    decimal TotalCostValue,
    /// <summary>Total sales value from this supplier.</summary>
    decimal TotalSalesValue,
    /// <summary>Total quantity from this supplier.</summary>
    double TotalQuantity);