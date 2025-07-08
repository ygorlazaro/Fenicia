namespace Fenicia.Auth.Domains.Address;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Common.Database;

using State.Data;

[Table(name: "addresses")]
public class AddressModel : BaseModel
{
    [Required]
    [MaxLength(length: 100)]
    public string Street { get; set; } = null!;

    [Required]
    [MaxLength(length: 10)]
    public string Number { get; set; } = null!;

    [MaxLength(length: 10)]
    public string Complement { get; set; } = null!;

    [Required]
    [MaxLength(length: 9)]
    public string ZipCode { get; set; } = null!;

    [Required]
    public Guid StateId
    {
        get; set;
    }

    [Required]
    [MaxLength(length: 30)]
    public string City { get; set; } = null!;

    [ForeignKey(nameof(AddressModel.StateId))]
    [JsonIgnore]
    public virtual StateModel State { get; set; } = null!;
}
