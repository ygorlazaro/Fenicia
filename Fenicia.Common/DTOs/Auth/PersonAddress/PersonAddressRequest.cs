using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.PersonAddress;

/// <summary>
/// Request DTO for associating a person with an address.
/// </summary>
public class PersonAddressRequest()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PersonAddressRequest"/> class with the specified person and address identifiers.
    /// </summary>
    /// <param name="personId">The unique identifier of the person.</param>
    /// <param name="addressId">The unique identifier of the address.</param>
    public PersonAddressRequest(Guid personId, Guid addressId)
        : this()
    {
        PersonId = personId;
        AddressId = addressId;
    }

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
}
