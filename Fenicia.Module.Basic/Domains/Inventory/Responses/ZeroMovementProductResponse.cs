namespace Fenicia.Module.Basic.Domains.Inventory.Responses;

/// <summary>
/// Response model for products with zero movement.
/// </summary>
public record ZeroMovementProductResponse(
    /// <summary>Product ID.</summary>
    Guid ProductId,
    /// <summary>Product name.</summary>
    string ProductName,
    /// <summary>Category name.</summary>
    string CategoryName,
    /// <summary>Supplier name.</summary>
    string? SupplierName,
    /// <summary>Current stock quantity.</summary>
    double CurrentStock,
    /// <summary>Total stock value.</summary>
    decimal StockValue,
    /// <summary>Date of last stock movement.</summary>
    DateTime? LastMovementDate,
    /// <summary>Number of days without movement.</summary>
    int DaysWithoutMovement);
