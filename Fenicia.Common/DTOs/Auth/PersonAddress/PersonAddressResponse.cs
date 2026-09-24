using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.PersonAddress;

/// <summary>
/// Response DTO containing person-address association details.
/// </summary>
public class PersonAddressResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PersonAddressResponse"/> class with the specified details.
    /// </summary>
    /// <param name="id">The unique identifier of the person-address association.</param>
    /// <param name="personId">The unique identifier of the person.</param>
    /// <param name="addressId">The unique identifier of the address.</param>
    /// <param name="personName">The name of the person.</param>
    public PersonAddressResponse(Guid id, Guid personId, Guid addressId, string personName)
        : this()
    {
        Id = id;
        PersonId = personId;
        AddressId = addressId;
        PersonName = personName;
    }

    /// <summary>
    /// Gets the unique identifier of the person-address association.
    /// </summary>
    [Required]
    public Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the unique identifier of the person.
    /// </summary>
    [Required]
    public Guid PersonId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the address.
    /// </summary>
    [Required]
    public Guid AddressId { get; set; }

    /// <summary>
    /// Gets or sets the name of the person.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string PersonName { get; set; } = string.Empty;
}
