using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.State;

/// <summary>
/// Response DTO containing state information.
/// </summary>
public class StateResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StateResponse"/> class with the specified state details.
    /// </summary>
    /// <param name="id">The unique identifier of the state.</param>
    /// <param name="name">The name of the state.</param>
    /// <param name="uf">The state's abbreviation (UF).</param>
    public StateResponse(Guid id, string name, string uf)
        : this()
    {
        Id = id;
        Name = name;
        Uf = uf;
    }

    /// <summary>
    /// Gets the unique identifier of the state.
    /// </summary>
    [Required]
    public Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the name of the state.
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the state's abbreviation (UF).
    /// </summary>
    [Required]
    [MaxLength(2)]
    public string Uf { get; set; } = string.Empty;
}
