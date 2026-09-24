using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Position;

/// <summary>
/// Request DTO for creating or updating a position.
/// </summary>
public class PositionRequest()
{
    /// <summary>
    /// Initializes a new instance with the specified name.
    /// </summary>
    /// <param name="name">The position name.</param>
    public PositionRequest(string name) : this()
    {
        Name = name;
    }

    /// <summary>Gets or sets the position name.</summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}