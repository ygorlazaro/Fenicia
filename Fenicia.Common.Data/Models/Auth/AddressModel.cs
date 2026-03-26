using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.Data.Models.Auth;

[Table("addresses", Schema = "auth")]
public class AddressModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Street { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string Number { get; set; } = null!;

    [MaxLength(20)]
    public string? Complement { get; set; }

    [MaxLength(50)]
    public string? Neighborhood { get; set; }

    [Required]
    [MaxLength(8)]
    public string ZipCode { get; set; } = null!;

    [Required]
    public Guid StateId { get; set; }

    [Required]
    [MaxLength(50)]
    public string City { get; set; } = null!;

    [MaxLength(50)]
    public string? Country { get; set; }

    public AddressType AddressType { get; set; } = AddressType.Both;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public bool IsDefault { get; set; }

    [MaxLength(500)]
    public string? Observation { get; set; }

    [ForeignKey(nameof(StateId))]
    public virtual StateModel State { get; set; } = null!;
}