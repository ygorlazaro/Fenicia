namespace Fenicia.Module.Basic.Domains.Product.Commands;

/// <summary>
///     Command record for deleting a product.
/// </summary>
public record DeleteProductCommand(
    /// <summary>
    /// Unique identifier of the product to delete.
    /// </summary>
    Guid Id);