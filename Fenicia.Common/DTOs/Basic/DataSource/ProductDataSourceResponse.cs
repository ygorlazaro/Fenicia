using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.DataSource;

/// <summary>
/// Response DTO for product data source (dropdowns, lookups).
/// </summary>
public class ProductDataSourceResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProductDataSourceResponse"/> class with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="name">The name of the product.</param>
    public ProductDataSourceResponse(Guid id, string name)
        : this()
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the product.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}