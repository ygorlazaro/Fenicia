using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data.Models;

[Table("addresses", Schema = "auth")]
public class AuthAddress : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Street { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Number { get; set; } = null!;

    [MaxLength(50)]
    public string Complement { get; set; } = null!;

    [Required]
    [MaxLength(8)]
    public string ZipCode { get; set; } = null!;

    [Required]
    public Guid StateId { get; set; }

    [Required]
    [MaxLength(50)]
    public string City { get; set; } = null!;

    [ForeignKey(nameof(StateId))]
    [JsonIgnore]
    public virtual AuthStateModel StateModel { get; set; } = null!;
}
