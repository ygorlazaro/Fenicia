namespace Fenicia.Common.DTOs.Basic.Supplier;

public class AddressDTO
{
    public AddressDTO()
    {
    }

    public AddressDTO(
        string street,
        string number,
        string? complement,
        string? neighborhood,
        string zipCode,
        Guid stateId,
        string city,
        string? country)
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

    public string Street { get; set; } = string.Empty;

    public string Number { get; set; } = string.Empty;

    public string? Complement { get; set; }

    public string? Neighborhood { get; set; }

    public string ZipCode { get; set; } = string.Empty;

    public Guid StateId { get; set; }

    public string City { get; set; } = string.Empty;

    public string? Country { get; set; }
}
