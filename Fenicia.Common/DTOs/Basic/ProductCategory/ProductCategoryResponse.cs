using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.ProductCategory;

/// <summary>
/// Response DTO containing product category data.
/// </summary>
/// <param name="id">The unique identifier of the category.</param>
/// <param name="name">The category name.</param>
public record ProductCategoryResponse(
    [Required] Guid Id,
    [Required] [MaxLength(50)] string Name);