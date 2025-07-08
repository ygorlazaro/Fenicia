namespace Fenicia.Auth.Domains.State.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Address;

using Common.Database;

[Table(name: "states")]
public class StateModel : BaseModel
{
    [Required]
    [MaxLength(length: 30)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(length: 2)]
    public string Uf { get; set; } = null!;

    [JsonIgnore]
    public virtual List<AddressModel> Addresses { get; set; } = null!;
}
