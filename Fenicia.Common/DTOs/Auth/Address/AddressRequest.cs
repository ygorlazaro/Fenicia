using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Address;

public class AddressRequest()
{
    public AddressRequest(
        string street,
        string number,
        string? complement,
        string? neighborhood,
        string zipCode,
        Guid stateId,
        string city,
        string? country)
        : this()
    {
        Street = street;
        Number = number;
        Complement = complement;
        Neighborhood = neighborhood;
        ZipCode = zipCode;
        StateId = stateId;
        City = city;
        Country = country;
    }

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
    public string ZipCode { get; set; } = string.Empty;

    [Required]
    public Guid StateId { get; set; }

    [Required]
    [MaxLength(50)]
    public string City { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Country { get; set; }
}
