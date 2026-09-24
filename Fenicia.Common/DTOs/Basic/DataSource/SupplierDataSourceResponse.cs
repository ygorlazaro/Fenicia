using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.DataSource;

/// <summary>
/// Response DTO for supplier data source (dropdowns, lookups).
/// </summary>
public class SupplierDataSourceResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SupplierDataSourceResponse"/> class with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the supplier.</param>
    /// <param name="name">The name of the supplier.</param>
    public SupplierDataSourceResponse(Guid id, string name)
        : this()
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the supplier.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the supplier.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}