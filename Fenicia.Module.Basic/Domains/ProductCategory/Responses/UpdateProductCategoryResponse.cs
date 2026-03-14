namespace Fenicia.Module.Basic.Domains.ProductCategory.Responses;

/// <summary>
///     Response record for an updated product category.
/// </summary>
public record UpdateProductCategoryResponse(
    /// <summary>
    /// Unique identifier of the category.
    /// </summary>
    Guid Id,
    /// <summary>
    /// Name of the category.
    /// </summary>
    string Name);