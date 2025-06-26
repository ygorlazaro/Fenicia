using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Auth.Domains.Address;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Domains.State.Data;

/// <summary>
/// Represents a state/province entity in the system.
/// </summary>
[Table("states")] // Maps the entity to the 'states' database table
public class StateModel : BaseModel
{
    /// <summary>
    /// Gets or sets the name of the state.
    /// </summary>
    [Required] // Ensures the Name field is not null
    [MaxLength(30)] // Limits the Name field to 30 characters
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the UF (Unidade Federativa) code of the state.
    /// </summary>
    [Required] // Ensures the UF field is not null
    [MaxLength(2)] // Limits the UF field to 2 characters
    public string Uf { get; set; } = null!;

    /// <summary>
    /// Gets or sets the list of addresses associated with this state.
    /// </summary>
    [JsonIgnore] // Prevents this property from being serialized to JSON
    public virtual List<AddressModel> Addresses { get; set; } = null!;
}
