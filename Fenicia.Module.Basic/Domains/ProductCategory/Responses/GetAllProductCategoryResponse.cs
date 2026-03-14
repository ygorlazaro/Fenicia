namespace Fenicia.Module.Basic.Domains.ProductCategory.Responses;

/// <summary>
///     Response record for a product category in a list.
/// </summary>
public record GetAllProductCategoryResponse(
    /// <summary>
    /// Unique identifier of the category.
    /// </summary>
    Guid Id,
    /// <summary>
    /// Name of the category.
    /// </summary>
    string Name);