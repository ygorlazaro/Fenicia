using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.Data.Models.Auth;

[Table("addresses", Schema = "auth")]
public sealed class AddressModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Street { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Number { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Complement { get; set; }

    [MaxLength(50)]
    public string? Neighborhood { get; set; }

    [Required]
    [MaxLength(8)]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "ZipCode deve conter exatamente 8 dígitos numéricos.")]
    public string? ZipCode
    {
        get;
        set => field = !string.IsNullOrWhiteSpace(value) ? new string([.. value.Where(char.IsDigit).Take(8)]) : null;
    }

    [Required]
    public Guid StateId { get; init; }

    [Required]
    [MaxLength(50)]
    public string City { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Country { get; set; }

    public AddressType AddressType { get; init; } = AddressType.Both;

    public double? Latitude { get; init; }

    public double? Longitude { get; init; }

    public bool IsDefault { get; init; }

    [MaxLength(500)]
    public string? Observation { get; init; }

    [ForeignKey(nameof(StateId))]
    public StateModel State { get; set; } = default!;
}