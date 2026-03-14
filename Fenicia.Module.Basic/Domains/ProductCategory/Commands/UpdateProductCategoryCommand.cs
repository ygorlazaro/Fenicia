namespace Fenicia.Module.Basic.Domains.ProductCategory.Commands;

/// <summary>
/// Command record for updating an existing product category.
/// </summary>
public record UpdateProductCategoryCommand(
    /// <summary>
    /// Unique identifier of the category to update.
    /// </summary>
    Guid Id,
    /// <summary>
    /// Updated name of the category.
    /// </summary>
    string Name);
