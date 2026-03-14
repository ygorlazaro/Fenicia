using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Auth;

[Table("addresses", Schema = "auth")]
public class AddressModel : BaseModel
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
    public virtual StateModel State { get; set; } = null!;
}