namespace Fenicia.Common.Database.Models.Auth;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Database;

[Table("states")]
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
