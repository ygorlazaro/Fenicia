using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Address;

public class AddressResponse()
{
    public AddressResponse(
        Guid id,
        string street,
        string number,
        string? complement,
        string? neighborhood,
        string zipCode,
        Guid stateId,
        string? stateName,
        string city,
        string? country)
        : this()
    {
        Id = id;
        Street = street;
        Number = number;
        Complement = complement;
        Neighborhood = neighborhood;
        ZipCode = zipCode;
        StateId = stateId;
        StateName = stateName;
        City = city;
        Country = country;
    }

    [Required]
    public Guid Id { get; set; }

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
    public string ZipCode { get; set; } = string.Empty;

    [Required]
    public Guid StateId { get; set; }

    [MaxLength(50)]
    public string? StateName { get; set; }

    [Required]
    [MaxLength(50)]
    public string City { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Country { get; set; }
}
