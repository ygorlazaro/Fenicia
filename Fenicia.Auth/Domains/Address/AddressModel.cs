namespace Fenicia.Auth.Domains.Address;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Common.Database;

using State.Data;

/// <summary>
///     Represents an address entity in the system
/// </summary>
[Table(name: "addresses")]
public class AddressModel : BaseModel
{
    /// <summary>
    ///     Gets or sets the street name
    /// </summary>
    [Required]
    [MaxLength(length: 100)]
    public string Street { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the address number
    /// </summary>
    [Required]
    [MaxLength(length: 10)]
    public string Number { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the address complement
    /// </summary>
    [MaxLength(length: 10)]
    public string Complement { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the ZIP code
    /// </summary>
    [Required]
    [MaxLength(length: 9)]
    public string ZipCode { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the state identifier
    /// </summary>
    [Required]
    public Guid StateId { get; set; }

    /// <summary>
    ///     Gets or sets the city name
    /// </summary>
    [Required]
    [MaxLength(length: 30)]
    public string City { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the associated state
    /// </summary>
    [ForeignKey(nameof(AddressModel.StateId))]
    [JsonIgnore]
    public virtual StateModel State { get; set; } = null!;
}
