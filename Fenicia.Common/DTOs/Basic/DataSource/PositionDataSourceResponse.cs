using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.DataSource;

/// <summary>
/// Response DTO for position data source (dropdowns, lookups).
/// </summary>
public class PositionDataSourceResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PositionDataSourceResponse"/> class with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the position.</param>
    /// <param name="name">The name of the position.</param>
    public PositionDataSourceResponse(Guid id, string name)
        : this()
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the position.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the position.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}