using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.DataSource;

/// <summary>
/// Response DTO for product category data source (dropdowns, lookups).
/// </summary>
public class ProductCategoryDataSourceResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProductCategoryDataSourceResponse"/> class with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the product category.</param>
    /// <param name="name">The name of the product category.</param>
    public ProductCategoryDataSourceResponse(Guid id, string name)
        : this()
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the product category.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the product category.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}