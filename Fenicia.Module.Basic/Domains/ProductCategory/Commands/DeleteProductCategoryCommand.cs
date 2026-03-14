namespace Fenicia.Module.Basic.Domains.ProductCategory.Commands;

/// <summary>
///     Command record for deleting a product category.
/// </summary>
public record DeleteProductCategoryCommand(
    /// <summary>
    /// Unique identifier of the category to delete.
    /// </summary>
    Guid Id);