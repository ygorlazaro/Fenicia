using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Fenicia.Common.Database;

namespace Fenicia.Auth.Contexts.Models;

[Table("states")]
public class StateModel : BaseModel
{
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(2)]
    public string Uf { get; set; } = null!;

    [JsonIgnore]
    public virtual List<AddressModel> Addresses { get; set; } = null!;
}