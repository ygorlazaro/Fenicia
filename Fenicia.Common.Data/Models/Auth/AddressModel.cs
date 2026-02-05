using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data.Models.Auth;

[Table("addresses")]
public class AddressModel : BaseModel
{
    [Required]
    [MaxLength(100)]
    public string Street { get; set; } = null!;

    [Required]
    [MaxLength(10)]
    public string Number { get; set; } = null!;

    [MaxLength(10)]
    public string Complement { get; set; } = null!;

    [Required]
    [MaxLength(9)]
    public string ZipCode { get; set; } = null!;

    [Required]
    public Guid StateId
    {
        get; set;
    }

    [Required]
    [MaxLength(30)]
    public string City { get; set; } = null!;

    [ForeignKey(nameof(StateId))]
    [JsonIgnore]
    public virtual StateModel State { get; set; } = null!;
}
