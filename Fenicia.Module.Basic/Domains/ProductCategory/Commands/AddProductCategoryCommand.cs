namespace Fenicia.Module.Basic.Domains.ProductCategory.Commands;

/// <summary>
///     Command record for creating a new product category.
/// </summary>
public record AddProductCategoryCommand(
    /// <summary>
    /// Unique identifier for the new category.
    /// </summary>
    Guid Id,
    /// <summary>
    /// Name of the category.
    /// </summary>
    string Name);