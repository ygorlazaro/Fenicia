using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.DataSource;

/// <summary>
/// Response DTO for customer data source (dropdowns, lookups).
/// </summary>
public class CustomerDataSourceResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerDataSourceResponse"/> class with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the customer.</param>
    /// <param name="name">The name of the customer.</param>
    public CustomerDataSourceResponse(Guid id, string name)
        : this()
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the customer.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the customer.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}