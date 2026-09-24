using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Address;

/// <summary>
/// Response DTO containing address details.
/// </summary>
public class AddressResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddressResponse"/> class with the specified address details.
    /// </summary>
    /// <param name="id">The unique identifier of the address.</param>
    /// <param name="street">The street name.</param>
    /// <param name="number">The street number.</param>
    /// <param name="complement">The address complement (optional).</param>
    /// <param name="neighborhood">The neighborhood (optional).</param>
    /// <param name="zipCode">The ZIP code.</param>
    /// <param name="stateId">The unique identifier of the state.</param>
    /// <param name="stateName">The name of the state (optional).</param>
    /// <param name="city">The city name.</param>
    /// <param name="country">The country name (optional).</param>
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

    /// <summary>
    /// Gets or sets the unique identifier of the address.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the street name.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the street number.
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the address complement.
    /// </summary>
    [MaxLength(20)]
    public string? Complement { get; set; }

    /// <summary>
    /// Gets or sets the neighborhood.
    /// </summary>
    [MaxLength(50)]
    public string? Neighborhood { get; set; }

    /// <summary>
    /// Gets or sets the ZIP code.
    /// </summary>
    [Required]
    [MaxLength(8)]
    public string ZipCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the state.
    /// </summary>
    [Required]
    public Guid StateId { get; set; }

    /// <summary>
    /// Gets or sets the name of the state.
    /// </summary>
    [MaxLength(50)]
    public string? StateName { get; set; }

    /// <summary>
    /// Gets or sets the city name.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country name.
    /// </summary>
    [MaxLength(50)]
    public string? Country { get; set; }
}
