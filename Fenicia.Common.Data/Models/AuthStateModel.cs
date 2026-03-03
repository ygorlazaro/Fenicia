using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data.Models;

[Table("states", Schema = "auth")]
public class AuthState : BaseModel
{
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(2)]
    public string Uf { get; set; } = null!;

    [JsonIgnore]
    public virtual List<AuthAddress> Addresses { get; set; } = null!;

    [JsonIgnore]
    public virtual List<BasicPersonModel> People { get; set; } = null!;
}
