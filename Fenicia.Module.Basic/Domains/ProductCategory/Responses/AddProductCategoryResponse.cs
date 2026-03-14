namespace Fenicia.Module.Basic.Domains.ProductCategory.Responses;

/// <summary>
/// Response record for a newly created product category.
/// </summary>
public record AddProductCategoryResponse(
    /// <summary>
    /// Unique identifier of the category.
    /// </summary>
    Guid Id,
    /// <summary>
    /// Name of the category.
    /// </summary>
    string Name);
