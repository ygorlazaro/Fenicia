namespace Fenicia.Module.Basic.Domains.Order.Commands;

/// <summary>
/// Command to specify an item in an order.
/// </summary>
public record OrderDetailCommand(
    /// <summary>Product ID.</summary>
    Guid ProductId,
    /// <summary>Unit price.</summary>
    decimal Price,
    /// <summary>Quantity ordered.</summary>
    double Quantity);
