namespace Fenicia.Module.Basic.Domains.Inventory.Responses;

/// <summary>
/// Response model for inventory item details.
/// </summary>
public record InventoryDetailResponse(
    /// <summary>Product ID.</summary>
    Guid Id,
    /// <summary>Product name.</summary>
    string Name,
    /// <summary>Current quantity in stock.</summary>
    double Quantity,
    /// <summary>Cost price per unit.</summary>
    decimal? CostPrice,
    /// <summary>Sales price per unit.</summary>
    decimal SalesPrice,
    /// <summary>Category ID.</summary>
    Guid CategoryId,
    /// <summary>Category name.</summary>
    string CategoryName);
