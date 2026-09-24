using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.ProductCategory;

/// <summary>
/// Request DTO for creating or updating a product category.
/// </summary>
public class ProductCategoryRequest()
{
    /// <summary>
    /// Initializes a new instance with the specified name.
    /// </summary>
    /// <param name="name">The category name.</param>
    public ProductCategoryRequest(string name) : this()
    {
        Name = name;
    }

    /// <summary>Gets or sets the category name.</summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}