namespace Fenicia.Module.Basic.Domains.ProductCategory.Responses;

/// <summary>
///     Response record for retrieving a single product category by ID.
/// </summary>
public record GetProductCategoryByIdResponse(
    /// <summary>
    /// Unique identifier of the category.
    /// </summary>
    Guid Id,
    /// <summary>
    /// Name of the category.
    /// </summary>
    string Name);